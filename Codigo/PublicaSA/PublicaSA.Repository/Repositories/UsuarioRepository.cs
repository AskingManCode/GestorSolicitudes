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

        public async Task<UsuarioResponse> Crear(UsuarioRequest usuario)
        {
            using (IDbConnection db = _connectionFactory.CreateConnection())
            {
                return await db.QuerySingleAsync<UsuarioResponse>(
                    "sp_Usuarios_Crear",
                    new
                    {
                        p_NombreCompleto = usuario.NombreCompleto,
                        p_Correo = usuario.Correo,
                        p_NombreUsuario = usuario.NombreUsuario,
                        p_Contrasena = usuario.Contrasena,
                        p_CodigoEstado = usuario.CodigoEstado
                    },
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public async Task<UsuarioResponse> CambiarEstado(int idUsuario, string codigoEstado)
        {
            using (IDbConnection db = _connectionFactory.CreateConnection())
            {
                return await db.QuerySingleAsync<UsuarioResponse>(
                    "sp_Usuarios_CambiarEstado",
                    new
                    {
                        p_IdUsuario = idUsuario,
                        p_CodigoEstado = codigoEstado
                    },
                    commandType: CommandType.StoredProcedure
                );
            }
        }
    }
}
