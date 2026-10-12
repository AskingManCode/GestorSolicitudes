using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using PublicaSA.Entities;
using PublicaSA.Repository.Interfaces;

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
    }
}
