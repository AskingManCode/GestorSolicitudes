using PublicaSA.Entities;
using PublicaSA.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Dapper;

namespace PublicaSA.Repository.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IDBConnectionFactory _connectionFactory;

        public UsuarioRepository(IDBConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<(IEnumerable<UsuarioResponse> ListaUsuarios, int TotalRegistros)> ListarUsuarios(int pagina, int tamanoPagina)
        {
            using (IDbConnection db = _connectionFactory.CreateConnection())
            {
                using (var multi = await db.QueryMultipleAsync(
                    "sp_Usuarios_Listar",
                    new { p_Pagina = pagina, p_TamanoPagina = tamanoPagina },
                    commandType: CommandType.StoredProcedure))
                {
                    var total = await multi.ReadFirstAsync<int>();
                    var listaUsuarios = await multi.ReadAsync<UsuarioResponse>();
                    return (listaUsuarios, total);
                }
            }
        }
    }
}
