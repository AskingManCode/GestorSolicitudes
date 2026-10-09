using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using PublicaSA.DataAccess.Interfaces;
using PublicaSA.Entities;

namespace PublicaSA.DataAccess.Repositories
{
    public class RepresentanteRepository : IRepresentanteRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public RepresentanteRepository(IDbConnectionFactory connectionFactory)
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
