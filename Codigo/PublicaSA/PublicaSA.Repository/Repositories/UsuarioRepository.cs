using Dapper;
using MySqlConnector;
using PublicaSA.Entities;
using PublicaSA.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async Task<UsuarioRequest?> ObtenerPorId(int idUsuario)
        {
            using (IDbConnection db = _connectionFactory.CreateConnection())
            {
                return await db.QueryFirstOrDefaultAsync<UsuarioRequest>(
                    "sp_Usuarios_Obtener",
                    new { p_IdUsuario = idUsuario },
                    commandType: CommandType.StoredProcedure
                );
            }
        }
        public async Task<UsuarioResponse> Editar(UsuarioEditRequest request)
        {
            using (IDbConnection db = _connectionFactory.CreateConnection())
            {
                return await db.QuerySingleAsync<UsuarioResponse>(
                    "sp_Usuarios_Editar",
                    new
                    {
                        p_IdUsuario = request.IdUsuario,
                        p_NombreCompleto = request.NombreCompleto,
                        p_Correo = request.Correo,
                        p_NombreUsuario = request.NombreUsuario,
                        p_CodigoEstado = request.CodigoEstado,
                        p_ContrasenaNueva = request.ContrasenaNueva
                    },
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public async Task<string> ObtenerContrasenaUsuario(int idUsuario)
        {
            using (IDbConnection db = _connectionFactory.CreateConnection())
            {
                return await db.QuerySingleAsync<string>(
                    "SELECT Contrasena FROM Usuarios WHERE IdUsuario = @IdUsuario",
                    new { IdUsuario = idUsuario }
                );
            }
        }

        public async Task<bool> Eliminar(int idUsuario)
        {
            using (IDbConnection db = _connectionFactory.CreateConnection())
            {
                try
                {
                    var filasAfectadas = await db.ExecuteAsync(
                        "sp_Usuarios_Eliminar",
                        new { p_IdUsuario = idUsuario },
                        commandType: CommandType.StoredProcedure
                    );

                    return filasAfectadas > 0;
                }
                catch (MySqlException ex) when (ex.Number == 1644)
                {
                    throw new InvalidOperationException(ex.Message);
                }

            }
        }

        public async Task<UsuarioLogin?> ObtenerPorNombreUsuarioAsync(string nombreUsuario)
        {
            using (IDbConnection db = _connectionFactory.CreateConnection())
            {
                return await db.QueryFirstOrDefaultAsync<UsuarioLogin>(
                    "sp_Usuarios_ObtenerPorNombreUsuario",
                    new { p_NombreUsuario = nombreUsuario },
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public async Task ActualizarIntentosYEstadoAsync(int idUsuario, int intentosFallidos, string? codigoEstado = null)
        {
            using (IDbConnection db = _connectionFactory.CreateConnection())
            {
                await db.ExecuteAsync(
                    "sp_Usuarios_ActualizarIntentosYEstado",
                    new
                    {
                        p_IdUsuario = idUsuario,
                        p_IntentosFallidos = intentosFallidos,
                        p_CodigoEstado = codigoEstado
                    },
                    commandType: CommandType.StoredProcedure
                );
            }
        }

    }
}
