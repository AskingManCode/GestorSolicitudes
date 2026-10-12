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
    }
}
