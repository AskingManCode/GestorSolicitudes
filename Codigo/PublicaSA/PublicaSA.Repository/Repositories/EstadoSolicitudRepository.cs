using PublicaSA.Entities;
using PublicaSA.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;

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
