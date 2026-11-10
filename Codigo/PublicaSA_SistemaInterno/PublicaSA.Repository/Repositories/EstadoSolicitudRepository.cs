using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using PublicaSA.Entities;
using PublicaSA.Repository.Interfaces;

namespace PublicaSA.Repository.Repositories
{
    public class EstadoSolicitudRepository : IEstadoSolicitudRepository
    {
        private readonly IDBConnectionFactory _connectionFactory;

        public EstadoSolicitudRepository(IDBConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<EstadoSolicitud>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM EstadosSolicitud WHERE Activo = TRUE ORDER BY CodigoEstado ASC;";
            return await connection.QueryAsync<EstadoSolicitud>(sql);
        }

        public async Task<EstadoSolicitud?> GetByCodigoAsync(string codigo)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM EstadosSolicitud WHERE CodigoEstado = @Codigo;";
            return await connection.QueryFirstOrDefaultAsync<EstadoSolicitud>(sql, new { Codigo = codigo });
        }

        public async Task<EstadoSolicitud?> GetByNombreAsync(string nombre)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM EstadosSolicitud WHERE Nombre = @Nombre;";
            return await connection.QueryFirstOrDefaultAsync<EstadoSolicitud>(sql, new { Nombre = nombre });
        }

        // HU SOL4: listado paginado (máximo 10 por página).
        public async Task<(IEnumerable<EstadoSolicitud> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
        {
            using var connection = _connectionFactory.CreateConnection();
            int offset = (pageNumber - 1) * pageSize;

            const string sql = @"
                SELECT * FROM EstadosSolicitud
                -- Primero los estados por defecto en el orden del ciclo de vida,
                -- luego los creados por los usuarios.
                ORDER BY
                    FIELD(CodigoEstado, 'NUEVA', 'RESPONDIDA', 'INICIADA', 'FINALIZADA', 'VENCIDA') = 0,
                    FIELD(CodigoEstado, 'NUEVA', 'RESPONDIDA', 'INICIADA', 'FINALIZADA', 'VENCIDA'),
                    FechaCreacion ASC,
                    CodigoEstado ASC
                LIMIT @Offset, @PageSize;

                SELECT COUNT(1) FROM EstadosSolicitud;";

            using var multi = await connection.QueryMultipleAsync(sql, new { Offset = offset, PageSize = pageSize });
            var items = await multi.ReadAsync<EstadoSolicitud>();
            int total = await multi.ReadSingleAsync<int>();

            return (items, total);
        }

        public async Task<bool> InsertAsync(EstadoSolicitud estado)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                INSERT INTO EstadosSolicitud (CodigoEstado, Nombre, Activo, FechaCreacion)
                VALUES (@CodigoEstado, @Nombre, TRUE, NOW());";

            int rows = await connection.ExecuteAsync(sql, estado);
            return rows > 0;
        }

        // El identificador es la llave primaria (y llave foránea en Solicitudes),
        // por eso solo se actualiza el nombre.
        public async Task<bool> UpdateAsync(EstadoSolicitud estado)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE EstadosSolicitud
                SET Nombre = @Nombre
                WHERE CodigoEstado = @CodigoEstado;";

            int rows = await connection.ExecuteAsync(sql, estado);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(string codigo)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "DELETE FROM EstadosSolicitud WHERE CodigoEstado = @Codigo;";
            int rows = await connection.ExecuteAsync(sql, new { Codigo = codigo });
            return rows > 0;
        }

        // Un estado está "asignado" si alguna solicitud lo usa.
        public async Task<bool> HasRelatedDataAsync(string codigo)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT COUNT(1) FROM Solicitudes WHERE CodigoEstado = @Codigo;";

            int count = await connection.ExecuteScalarAsync<int>(sql, new { Codigo = codigo });
            return count > 0;
        }
    }
}
