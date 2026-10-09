using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using PublicaSA.DataAccess.Interfaces;
using PublicaSA.Entities;

namespace PublicaSA.DataAccess.Repositories
{
    public class EstadoSolicitudRepository : IEstadoSolicitudRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public EstadoSolicitudRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<EstadoSolicitud>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM EstadosSolicitud ORDER BY IdEstado ASC;";
            return await connection.QueryAsync<EstadoSolicitud>(sql);
        }

        public async Task<EstadoSolicitud?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM EstadosSolicitud WHERE IdEstado = @Id;";
            return await connection.QueryFirstOrDefaultAsync<EstadoSolicitud>(sql, new { Id = id });
        }

        public async Task<EstadoSolicitud?> GetByNombreAsync(string nombre)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM EstadosSolicitud WHERE Nombre = @Nombre;";
            return await connection.QueryFirstOrDefaultAsync<EstadoSolicitud>(sql, new { Nombre = nombre });
        }
    }
}
