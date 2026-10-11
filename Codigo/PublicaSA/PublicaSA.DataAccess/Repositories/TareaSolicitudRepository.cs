using Dapper;
using PublicaSA.DataAccess.Interfaces;
using PublicaSA.Entities;
using System.Text.Json;

namespace PublicaSA.DataAccess.Repositories
{
    public class TareaSolicitudRepository : ITareaSolicitudRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public TareaSolicitudRepository(
            IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // Consulta los identificadores y nombres para el formulario de tareas.
        public async Task<IEnumerable<UsuarioSeleccion>> ListarUsuariosAsync()
        {
            const string sql = """
        SELECT IdUsuario, NombreCompleto
        FROM Usuarios
        ORDER BY NombreCompleto, IdUsuario;
        """;

            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryAsync<UsuarioSeleccion>(sql);
        }

        // Obtiene hasta diez tareas de la solicitud por página.
        public async Task<IEnumerable<TareaSolicitud>>
            ListarPorSolicitudAsync(int idSolicitud, int pagina)
        {
            if (idSolicitud <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(idSolicitud));
            }

            if (pagina < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pagina));
            }

            const int tamanoPagina = 10;
            int desplazamiento = checked((pagina - 1) * tamanoPagina);

            const string sql = """
                SELECT IdTarea, IdSolicitud, IdUsuario,
                       Fecha, Descripcion, HorasInvertidas
                FROM TareasSolicitud
                WHERE IdSolicitud = @IdSolicitud
                ORDER BY Fecha DESC, IdTarea DESC
                LIMIT @TamanoPagina OFFSET @Desplazamiento;
                """;

            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryAsync<TareaSolicitud>(
                sql,
                new
                {
                    IdSolicitud = idSolicitud,
                    TamanoPagina = tamanoPagina,
                    Desplazamiento = desplazamiento
                });
        }

        // Cuenta las tareas para calcular las páginas del listado.
        public async Task<int> ContarPorSolicitudAsync(int idSolicitud)
        {
            if (idSolicitud <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(idSolicitud));
            }

            const string sql = """
                SELECT COUNT(*)
                FROM TareasSolicitud
                WHERE IdSolicitud = @IdSolicitud;
                """;

            using var connection = _connectionFactory.CreateConnection();

            return await connection.ExecuteScalarAsync<int>(
                sql,
                new { IdSolicitud = idSolicitud });
        }

        // Guarda la tarea y su bitácora en una misma transacción.
        public async Task<int> CrearAsync(
            TareaSolicitud tarea,
            int idUsuarioActual)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                const string sql = """
                    INSERT INTO TareasSolicitud
                        (IdSolicitud, IdUsuario, Fecha,
                         Descripcion, HorasInvertidas)
                    VALUES
                        (@IdSolicitud, @IdUsuario, @Fecha,
                         @Descripcion, @HorasInvertidas);

                    SELECT LAST_INSERT_ID();
                    """;

                var idTarea = await connection.ExecuteScalarAsync<int>(
                    sql,
                    new
                    {
                        tarea.IdSolicitud,
                        tarea.IdUsuario,
                        Fecha = tarea.Fecha.Date,
                        tarea.Descripcion,
                        tarea.HorasInvertidas
                    },
                    transaction);

                var registroNuevo = new TareaSolicitud
                {
                    IdTarea = idTarea,
                    IdSolicitud = tarea.IdSolicitud,
                    IdUsuario = tarea.IdUsuario,
                    Fecha = tarea.Fecha.Date,
                    Descripcion = tarea.Descripcion,
                    HorasInvertidas = tarea.HorasInvertidas
                };

                const string sqlBitacora = """
                    INSERT INTO Bitacoras
                        (IdUsuario, TipoAccion, Descripcion)
                    VALUES
                        (@IdUsuario, @TipoAccion, @Descripcion);
                    """;

                await connection.ExecuteAsync(
                    sqlBitacora,
                    new
                    {
                        IdUsuario = idUsuarioActual,
                        TipoAccion = "CREAR",
                        Descripcion = "El usuario crea una tarea. "
                            + JsonSerializer.Serialize(registroNuevo)
                    },
                    transaction);

                transaction.Commit();
                return idTarea;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // Obtiene la tarea dentro de la solicitud seleccionada.
        public async Task<TareaSolicitud?> ObtenerPorIdAsync(
            int idTarea,
            int idSolicitud)
        {
            if (idTarea <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(idTarea));
            }

            if (idSolicitud <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(idSolicitud));
            }

            const string sql = """
                SELECT IdTarea, IdSolicitud, IdUsuario,
                       Fecha, Descripcion, HorasInvertidas
                FROM TareasSolicitud
                WHERE IdTarea = @IdTarea
                  AND IdSolicitud = @IdSolicitud;
                """;

            using var connection = _connectionFactory.CreateConnection();

            return await connection.QuerySingleOrDefaultAsync<TareaSolicitud>(
                sql,
                new
                {
                    IdTarea = idTarea,
                    IdSolicitud = idSolicitud
                });
        }

        // Actualiza la tarea y conserva ambos registros en la bitácora.
        public async Task<bool> ActualizarAsync(
            TareaSolicitud tarea,
            int idUsuarioActual)
        {
            ArgumentNullException.ThrowIfNull(tarea);

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                const string sqlAnterior = """
                    SELECT IdTarea, IdSolicitud, IdUsuario,
                           Fecha, Descripcion, HorasInvertidas
                    FROM TareasSolicitud
                    WHERE IdTarea = @IdTarea
                      AND IdSolicitud = @IdSolicitud
                    FOR UPDATE;
                    """;

                // Bloquea la fila mientras se actualiza y registra el cambio.
                var anterior = await connection
                    .QuerySingleOrDefaultAsync<TareaSolicitud>(
                        sqlAnterior,
                        new { tarea.IdTarea, tarea.IdSolicitud },
                        transaction);

                if (anterior is null)
                {
                    transaction.Rollback();
                    return false;
                }

                const string sqlActualizar = """
                    UPDATE TareasSolicitud
                    SET IdUsuario = @IdUsuario,
                        Fecha = @Fecha,
                        Descripcion = @Descripcion,
                        HorasInvertidas = @HorasInvertidas
                    WHERE IdTarea = @IdTarea
                      AND IdSolicitud = @IdSolicitud;
                    """;

                var actual = new TareaSolicitud
                {
                    IdTarea = anterior.IdTarea,
                    IdSolicitud = anterior.IdSolicitud,
                    IdUsuario = tarea.IdUsuario,
                    Fecha = tarea.Fecha.Date,
                    Descripcion = tarea.Descripcion,
                    HorasInvertidas = tarea.HorasInvertidas
                };

                await connection.ExecuteAsync(
                    sqlActualizar,
                    actual,
                    transaction);

                const string sqlBitacora = """
                    INSERT INTO Bitacoras
                        (IdUsuario, TipoAccion, Descripcion)
                    VALUES
                        (@IdUsuario, @TipoAccion, @Descripcion);
                    """;

                var descripcion = "El usuario actualiza una tarea. "
                    + JsonSerializer.Serialize(new
                    {
                        Anterior = anterior,
                        Actual = actual
                    });

                await connection.ExecuteAsync(
                    sqlBitacora,
                    new
                    {
                        IdUsuario = idUsuarioActual,
                        TipoAccion = "ACTUALIZAR",
                        Descripcion = descripcion
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

        // Elimina la tarea y registra sus datos antes de borrarla.
        public async Task<bool> EliminarAsync(
            int idTarea,
            int idSolicitud,
            int idUsuarioActual)
        {
            if (idTarea <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(idTarea));
            }

            if (idSolicitud <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(idSolicitud));
            }

            if (idUsuarioActual <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(idUsuarioActual));
            }

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                const string sqlConsultar = """
                    SELECT IdTarea, IdSolicitud, IdUsuario,
                           Fecha, Descripcion, HorasInvertidas
                    FROM TareasSolicitud
                    WHERE IdTarea = @IdTarea
                      AND IdSolicitud = @IdSolicitud
                    FOR UPDATE;
                    """;

                // Conserva los datos y bloquea la fila durante la eliminación.
                var tarea = await connection
                    .QuerySingleOrDefaultAsync<TareaSolicitud>(
                        sqlConsultar,
                        new
                        {
                            IdTarea = idTarea,
                            IdSolicitud = idSolicitud
                        },
                        transaction);

                if (tarea is null)
                {
                    transaction.Rollback();
                    return false;
                }

                const string sqlEliminar = """
                    DELETE FROM TareasSolicitud
                    WHERE IdTarea = @IdTarea
                      AND IdSolicitud = @IdSolicitud;
                    """;

                await connection.ExecuteAsync(
                    sqlEliminar,
                    new
                    {
                        IdTarea = idTarea,
                        IdSolicitud = idSolicitud
                    },
                    transaction);

                const string sqlBitacora = """
                    INSERT INTO Bitacoras
                        (IdUsuario, TipoAccion, Descripcion)
                    VALUES
                        (@IdUsuario, @TipoAccion, @Descripcion);
                    """;

                await connection.ExecuteAsync(
                    sqlBitacora,
                    new
                    {
                        IdUsuario = idUsuarioActual,
                        TipoAccion = "ELIMINAR",
                        Descripcion = "El usuario elimina una tarea. "
                            + JsonSerializer.Serialize(tarea)
                    },
                    transaction);

                transaction.Commit();
                return true;
            }
            catch
            {
                // Deshace los cambios si falla la eliminación o la bitácora.
                transaction.Rollback();
                throw;
            }
        }
    }
}