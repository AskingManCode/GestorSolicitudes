using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using PublicaSA.Repository.Interfaces;
using PublicaSA.Entities;

namespace PublicaSA.Repository.Repositories
{
    public class RepresentanteRepository : IRepresentanteRepository
    {
        private readonly IDBConnectionFactory _connectionFactory;

        public RepresentanteRepository(IDBConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Representante>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM Representantes ORDER BY Nombre ASC;";
            return await connection.QueryAsync<Representante>(sql);
        }

        public async Task<Representante?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM Representantes WHERE IdRepresentante = @Id;";
            return await connection.QueryFirstOrDefaultAsync<Representante>(sql, new { Id = id });
        }

        public async Task<(IEnumerable<Representante> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
        {
            using var connection = _connectionFactory.CreateConnection();
            int offset = (pageNumber - 1) * pageSize;

            const string sql = @"
                SELECT * FROM Representantes
                ORDER BY Nombre ASC
                LIMIT @Offset, @PageSize;

                SELECT COUNT(1) FROM Representantes;";

            using var multi = await connection.QueryMultipleAsync(sql, new { Offset = offset, PageSize = pageSize });
            var items = await multi.ReadAsync<Representante>();
            int total = await multi.ReadSingleAsync<int>();

            return (items, total);
        }

        public async Task<int> InsertAsync(Representante representante)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                INSERT INTO Representantes (Nombre, Email, FechaCreacion)
                VALUES (@Nombre, @Email, NOW());
                SELECT LAST_INSERT_ID();";

            return await connection.ExecuteScalarAsync<int>(sql, representante);
        }

        public async Task<bool> UpdateAsync(Representante representante)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE Representantes
                SET Nombre = @Nombre,
                    Email = @Email
                WHERE IdRepresentante = @IdRepresentante;";

            int rows = await connection.ExecuteAsync(sql, representante);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "DELETE FROM Representantes WHERE IdRepresentante = @Id;";
            int rows = await connection.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }

        public async Task<bool> HasRelatedDataAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT COUNT(1) FROM Solicitudes WHERE IdRepresentante = @Id;";

            int count = await connection.ExecuteScalarAsync<int>(sql, new { Id = id });
            return count > 0;
        }
    }
}
