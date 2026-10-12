using Dapper;
using PublicaSA.Entities;
using PublicaSA.Repository.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace PublicaSA.Repository.Repositories
{
    internal class EstadosUsuarioRepository : IEstadosUsuarioRepository
    {
        private readonly IDBConnectionFactory _connectionFactory;

        public EstadosUsuarioRepository(IDBConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<EstadoUsuario>> ListarEstadosUsuario()
        {
            using (IDbConnection db = _connectionFactory.CreateConnection())
            {
                var estados = await db.QueryAsync<EstadoUsuario>(
                    "sp_EstadosUsuario_Listar",
                    commandType: CommandType.StoredProcedure
                );

                return estados;
            }
        }
    }
}