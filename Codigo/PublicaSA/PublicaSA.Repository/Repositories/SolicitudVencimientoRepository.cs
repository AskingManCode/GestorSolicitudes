using System.Text.Json;
using Dapper;
using PublicaSA.Entities;
using PublicaSA.Repository.Interfaces;

namespace PublicaSA.Repository.Repositories
{
    public class SolicitudVencimientoRepository
        : ISolicitudVencimientoRepository
    {
        private readonly IDBConnectionFactory _connectionFactory;

        public SolicitudVencimientoRepository(
            IDBConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // Consulta las solicitudes que todavía necesitan revisión.
        public async Task<IEnumerable<Solicitud>> ListarPendientesAsync()
        {
            const string sql = """
                SELECT IdSolicitud, ConsecutivoOficio,
                       DocumentoRespuesta, DocumentoInicio,
                       Titulo, Descripcion, IdRepresentante,
                       Observaciones, CodigoEstado,
                       FechaIngreso, FechaRespuesta, FechaInicio,
                       Activo, FechaCreacion
                FROM Solicitudes
                WHERE FechaRespuesta IS NULL
                  AND CodigoEstado <> 'VENCIDA'
                ORDER BY IdSolicitud;
                """;

            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryAsync<Solicitud>(sql);
        }

        public async Task<bool> MarcarVencidaAsync(
            int idSolicitud,
            DateTime fechaActual)
        {
            if (idSolicitud <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(idSolicitud));
            }

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                const string sqlConsultar = """
                    SELECT IdSolicitud, ConsecutivoOficio,
                           DocumentoRespuesta, DocumentoInicio,
                           Titulo, Descripcion, IdRepresentante,
                           Observaciones, CodigoEstado,
                           FechaIngreso, FechaRespuesta, FechaInicio,
                           Activo, FechaCreacion
                    FROM Solicitudes
                    WHERE IdSolicitud = @IdSolicitud
                    FOR UPDATE;
                    """;

                // Bloquea la fila y lee sus datos actuales antes del cambio.
                var solicitud = await connection
                    .QuerySingleOrDefaultAsync<Solicitud>(
                        sqlConsultar,
                        new { IdSolicitud = idSolicitud },
                        transaction);

                if (solicitud is null
                    || solicitud.FechaRespuesta.HasValue
                    || solicitud.CodigoEstado == "VENCIDA")
                {
                    transaction.Rollback();
                    return false;
                }

                DateTime fechaLimite =
                    Solicitud.CalcularQuintoDiaHabil(
                        solicitud.FechaIngreso);

                // En el quinto día todavía puede responder; vence después.
                if (fechaActual.Date <= fechaLimite)
                {
                    transaction.Rollback();
                    return false;
                }

                var anterior = ObtenerDatosRegistro(solicitud);

                const string sqlActualizar = """
                    UPDATE Solicitudes
                    SET CodigoEstado = 'VENCIDA'
                    WHERE IdSolicitud = @IdSolicitud;
                    """;

                await connection.ExecuteAsync(
                    sqlActualizar,
                    new { IdSolicitud = idSolicitud },
                    transaction);

                solicitud.CodigoEstado = "VENCIDA";
                var actual = ObtenerDatosRegistro(solicitud);

                const string sqlBitacora = """
                    INSERT INTO Bitacoras
                        (FechaBitacora, UsuarioEjecuta, Accion, Detalle)
                    VALUES
                        (NOW(), @UsuarioEjecuta, @Accion, @Detalle);
                    """;

                await connection.ExecuteAsync(
                    sqlBitacora,
                    new
                    {
                        UsuarioEjecuta = "Sistema",
                        Accion = "ACTUALIZAR",
                        Detalle =
                            "El proceso automático marca una solicitud vencida. "
                            + JsonSerializer.Serialize(new
                            {
                                Anterior = anterior,
                                Actual = actual
                            })
                    },
                    transaction);

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // Incluye los campos almacenados, sin propiedades calculadas de pantalla.
        private static object ObtenerDatosRegistro(Solicitud solicitud)
        {
            return new
            {
                solicitud.IdSolicitud,
                solicitud.ConsecutivoOficio,
                solicitud.DocumentoRespuesta,
                solicitud.DocumentoInicio,
                solicitud.Titulo,
                solicitud.Descripcion,
                solicitud.IdRepresentante,
                solicitud.Observaciones,
                solicitud.CodigoEstado,
                solicitud.FechaIngreso,
                solicitud.FechaRespuesta,
                solicitud.FechaInicio,
                solicitud.Activo,
                solicitud.FechaCreacion
            };
        }
    }
}