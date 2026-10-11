using Dapper;
using PublicaSA.DataAccess.Interfaces;
using PublicaSA.Entities;
using System.Text.Json;

namespace PublicaSA.DataAccess.Repositories
{
    public class SolicitudVencimientoRepository
        : ISolicitudVencimientoRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public SolicitudVencimientoRepository(
            IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<SolicitudPendienteVencimiento>>
            ListarPendientesAsync()
        {
            const string sql = """
                SELECT
                    s.IdSolicitud,
                    s.FechaIngreso,
                    s.IdEstadoSolicitud
                FROM Solicitudes AS s
                INNER JOIN EstadosSolicitud AS e
                    ON e.IdEstadoSolicitud = s.IdEstadoSolicitud
                WHERE s.FechaRespuesta IS NULL
                  AND e.Nombre <> 'Vencida'
                ORDER BY s.IdSolicitud;
                """;

            using var connection = _connectionFactory.CreateConnection();

            return await connection
                .QueryAsync<SolicitudPendienteVencimiento>(sql);
        }

        public async Task<bool> MarcarVencidaAsync(
            int idSolicitud,
            DateTime fechaIngresoEsperada,
            DateTime fechaVencimiento,
            DateTime fechaActual)
        {
            if (idSolicitud <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(idSolicitud));
            }

            // La fecha límite todavía permite responder durante ese día.
            if (fechaActual.Date <= fechaVencimiento.Date)
            {
                return false;
            }

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                const string sqlEstado = """
                    SELECT IdEstadoSolicitud
                    FROM EstadosSolicitud
                    WHERE Nombre = 'Vencida';
                    """;

                int? idEstadoVencida =
                    await connection.QuerySingleOrDefaultAsync<int?>(
                        sqlEstado,
                        transaction: transaction);

                if (!idEstadoVencida.HasValue)
                {
                    throw new InvalidOperationException(
                        "No existe el estado Vencida.");
                }

                const string sqlConsultar = """
                    SELECT *
                    FROM Solicitudes
                    WHERE IdSolicitud = @IdSolicitud
                    FOR UPDATE;
                    """;

                // Bloquea la fila y conserva todos sus datos para la bitácora.
                var fila = await connection.QuerySingleOrDefaultAsync(
                    sqlConsultar,
                    new { IdSolicitud = idSolicitud },
                    transaction);

                if (fila is null)
                {
                    transaction.Rollback();
                    return false;
                }

                var anterior = new Dictionary<string, object>(
                    (IDictionary<string, object>)fila);

                DateTime fechaIngreso =
                    Convert.ToDateTime(anterior["FechaIngreso"]).Date;

                int idEstadoActual =
                    Convert.ToInt32(anterior["IdEstadoSolicitud"]);

                // Evita vencer una solicitud respondida o modificada entretanto.
                if (anterior["FechaRespuesta"] is not null ||
                    idEstadoActual == idEstadoVencida.Value ||
                    fechaIngreso != fechaIngresoEsperada.Date)
                {
                    transaction.Rollback();
                    return false;
                }

                const string sqlActualizar = """
                    UPDATE Solicitudes
                    SET IdEstadoSolicitud = @IdEstadoSolicitud,
                        FechaVencimiento = @FechaVencimiento
                    WHERE IdSolicitud = @IdSolicitud;
                    """;

                await connection.ExecuteAsync(
                    sqlActualizar,
                    new
                    {
                        IdSolicitud = idSolicitud,
                        IdEstadoSolicitud = idEstadoVencida.Value,
                        FechaVencimiento = fechaVencimiento.Date
                    },
                    transaction);

                var actual = new Dictionary<string, object>(anterior)
                {
                    ["IdEstadoSolicitud"] = idEstadoVencida.Value,
                    ["FechaVencimiento"] = fechaVencimiento.Date
                };

                const string sqlBitacora = """
                    INSERT INTO Bitacoras
                        (IdUsuario, TipoAccion, Descripcion)
                    VALUES
                        (NULL, 'ACTUALIZAR', @Descripcion);
                    """;

                // Es una acción automática, sin un usuario conectado.
                await connection.ExecuteAsync(
                    sqlBitacora,
                    new
                    {
                        Descripcion =
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
    }
}