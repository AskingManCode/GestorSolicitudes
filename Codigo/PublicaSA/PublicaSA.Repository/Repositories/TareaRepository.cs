using System.Data;
using System.Text.Json;
using Dapper;
using PublicaSA.Entities;
using PublicaSA.Repository.Interfaces;

namespace PublicaSA.Repository.Repositories
{
    public class TareaRepository : ITareaRepository
    {
        private readonly IDBConnectionFactory _connectionFactory;

        public TareaRepository(IDBConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // Consulta únicamente los datos necesarios para seleccionar al usuario.
        public async Task<IEnumerable<Usuario>> ListarUsuariosAsync()
        {
            const string sql = """
                SELECT IdUsuario, NombreUsuario, NombreCompleto
                FROM Usuarios
                ORDER BY NombreCompleto, IdUsuario;
                """;

            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryAsync<Usuario>(sql);
        }

        // Obtiene hasta diez tareas de la solicitud por página.
        public async Task<IEnumerable<Tarea>> ListarPorSolicitudAsync(
            int idSolicitud,
            int pagina)
        {
            if (idSolicitud <= 0)
                throw new ArgumentOutOfRangeException(nameof(idSolicitud));

            if (pagina < 1)
                throw new ArgumentOutOfRangeException(nameof(pagina));

            const int tamanoPagina = 10;
            int desplazamiento = checked((pagina - 1) * tamanoPagina);

            const string sql = """
                SELECT t.IdTarea, t.IdSolicitud, t.IdUsuario,
                       t.Descripcion, t.Fecha, t.HorasInvertidas,
                       t.FechaCreacion,
                       u.NombreCompleto AS NombreUsuario
                FROM Tareas t
                INNER JOIN Usuarios u ON u.IdUsuario = t.IdUsuario
                WHERE t.IdSolicitud = @IdSolicitud
                ORDER BY t.Fecha DESC, t.IdTarea DESC
                LIMIT @TamanoPagina OFFSET @Desplazamiento;
                """;

            using var connection = _connectionFactory.CreateConnection();

            return await connection.QueryAsync<Tarea>(
                sql,
                new
                {
                    IdSolicitud = idSolicitud,
                    TamanoPagina = tamanoPagina,
                    Desplazamiento = desplazamiento
                });
        }

        public async Task<int> ContarPorSolicitudAsync(int idSolicitud)
        {
            if (idSolicitud <= 0)
                throw new ArgumentOutOfRangeException(nameof(idSolicitud));

            const string sql = """
                SELECT COUNT(*)
                FROM Tareas
                WHERE IdSolicitud = @IdSolicitud;
                """;

            using var connection = _connectionFactory.CreateConnection();

            return await connection.ExecuteScalarAsync<int>(
                sql,
                new { IdSolicitud = idSolicitud });
        }

        public async Task<Tarea?> ObtenerPorIdAsync(
            int idTarea,
            int idSolicitud)
        {
            const string sql = """
                SELECT IdTarea, IdSolicitud, IdUsuario,
                       Descripcion, Fecha, HorasInvertidas,
                       FechaCreacion
                FROM Tareas
                WHERE IdTarea = @IdTarea
                  AND IdSolicitud = @IdSolicitud;
                """;

            using var connection = _connectionFactory.CreateConnection();

            return await connection.QuerySingleOrDefaultAsync<Tarea>(
                sql,
                new
                {
                    IdTarea = idTarea,
                    IdSolicitud = idSolicitud
                });
        }

        // Guarda la tarea y su bitácora en una misma transacción.
        public async Task<int> CrearAsync(
            Tarea tarea,
            string usuarioActual)
        {
            ArgumentNullException.ThrowIfNull(tarea);
            ValidarUsuarioActual(usuarioActual);

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                const string sql = """
                    INSERT INTO Tareas
                        (IdSolicitud, IdUsuario, Descripcion,
                         Fecha, HorasInvertidas)
                    VALUES
                        (@IdSolicitud, @IdUsuario, @Descripcion,
                         @Fecha, @HorasInvertidas);

                    SELECT LAST_INSERT_ID();
                    """;

                int idTarea = await connection.ExecuteScalarAsync<int>(
                    sql,
                    new
                    {
                        tarea.IdSolicitud,
                        tarea.IdUsuario,
                        tarea.Descripcion,
                        Fecha = tarea.Fecha.Date,
                        tarea.HorasInvertidas
                    },
                    transaction);

                var nueva = await LeerParaCambioAsync(
                    connection,
                    transaction,
                    idTarea,
                    tarea.IdSolicitud)
                    ?? throw new InvalidOperationException(
                        "No se pudo recuperar la tarea creada.");

                await RegistrarCambioAsync(
                    connection,
                    transaction,
                    usuarioActual,
                    "CREAR",
                    "El usuario crea una tarea. "
                        + JsonSerializer.Serialize(nueva));

                transaction.Commit();
                return idTarea;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // Conserva los datos anteriores y actuales en la bitácora.
        public async Task<bool> ActualizarAsync(
            Tarea tarea,
            string usuarioActual)
        {
            ArgumentNullException.ThrowIfNull(tarea);
            ValidarUsuarioActual(usuarioActual);

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                var anterior = await LeerParaCambioAsync(
                    connection,
                    transaction,
                    tarea.IdTarea,
                    tarea.IdSolicitud);

                if (anterior is null)
                {
                    transaction.Rollback();
                    return false;
                }

                const string sql = """
                    UPDATE Tareas
                    SET IdUsuario = @IdUsuario,
                        Descripcion = @Descripcion,
                        Fecha = @Fecha,
                        HorasInvertidas = @HorasInvertidas
                    WHERE IdTarea = @IdTarea
                      AND IdSolicitud = @IdSolicitud;
                    """;

                await connection.ExecuteAsync(
                    sql,
                    new
                    {
                        tarea.IdTarea,
                        tarea.IdSolicitud,
                        tarea.IdUsuario,
                        tarea.Descripcion,
                        Fecha = tarea.Fecha.Date,
                        tarea.HorasInvertidas
                    },
                    transaction);

                var actual = await LeerParaCambioAsync(
                    connection,
                    transaction,
                    tarea.IdTarea,
                    tarea.IdSolicitud)
                    ?? throw new InvalidOperationException(
                        "No se pudo recuperar la tarea actualizada.");

                await RegistrarCambioAsync(
                    connection,
                    transaction,
                    usuarioActual,
                    "ACTUALIZAR",
                    "El usuario actualiza una tarea. "
                        + JsonSerializer.Serialize(new
                        {
                            Anterior = anterior,
                            Actual = actual
                        }));

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // Registra los datos eliminados antes de confirmar la transacción.
        public async Task<bool> EliminarAsync(
            int idTarea,
            int idSolicitud,
            string usuarioActual)
        {
            ValidarUsuarioActual(usuarioActual);

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                var anterior = await LeerParaCambioAsync(
                    connection,
                    transaction,
                    idTarea,
                    idSolicitud);

                if (anterior is null)
                {
                    transaction.Rollback();
                    return false;
                }

                const string sql = """
                    DELETE FROM Tareas
                    WHERE IdTarea = @IdTarea
                      AND IdSolicitud = @IdSolicitud;
                    """;

                await connection.ExecuteAsync(
                    sql,
                    new
                    {
                        IdTarea = idTarea,
                        IdSolicitud = idSolicitud
                    },
                    transaction);

                await RegistrarCambioAsync(
                    connection,
                    transaction,
                    usuarioActual,
                    "ELIMINAR",
                    "El usuario elimina una tarea. "
                        + JsonSerializer.Serialize(anterior));

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        // Bloquea la fila mientras se modifica y registra el cambio.
        private static Task<Tarea?> LeerParaCambioAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            int idTarea,
            int idSolicitud)
        {
            const string sql = """
                SELECT IdTarea, IdSolicitud, IdUsuario,
                       Descripcion, Fecha, HorasInvertidas,
                       FechaCreacion
                FROM Tareas
                WHERE IdTarea = @IdTarea
                  AND IdSolicitud = @IdSolicitud
                FOR UPDATE;
                """;

            return connection.QuerySingleOrDefaultAsync<Tarea>(
                sql,
                new
                {
                    IdTarea = idTarea,
                    IdSolicitud = idSolicitud
                },
                transaction);
        }

        // Comparte la transacción para guardar juntos el cambio y la bitácora.
        private static Task<int> RegistrarCambioAsync(
            IDbConnection connection,
            IDbTransaction transaction,
            string usuario,
            string accion,
            string detalle)
        {
            const string sql = """
                INSERT INTO Bitacoras
                    (FechaBitacora, UsuarioEjecuta, Accion, Detalle)
                VALUES
                    (NOW(), @UsuarioEjecuta, @Accion, @Detalle);
                """;

            return connection.ExecuteAsync(
                sql,
                new
                {
                    UsuarioEjecuta = usuario,
                    Accion = accion,
                    Detalle = detalle
                },
                transaction);
        }

        private static void ValidarUsuarioActual(string usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario))
            {
                throw new ArgumentException(
                    "Se requiere identificar al usuario actual.",
                    nameof(usuario));
            }
        }
    }
}