using System;
using System.Collections.Generic;
using System.Text;
using PublicaSA.Entities;

namespace PublicaSA.Repository.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<(IEnumerable<UsuarioResponse> ListaUsuarios, int TotalRegistros)> ListarUsuarios(int pagina, int tamanoPagina);
        Task<UsuarioResponse> Crear(UsuarioRequest usuario);
        Task<UsuarioResponse> CambiarEstado(int idUsuario, string codigoEstado);
        Task<UsuarioRequest?> ObtenerPorId(int idUsuario);
        Task<UsuarioResponse> Editar(UsuarioEditRequest request);
        Task<string> ObtenerContrasenaUsuario(int idUsuario);
        Task<bool> Eliminar(int idUsuario);
        Task<UsuarioLogin?> ObtenerPorNombreUsuarioAsync(string nombreUsuario);
        Task ActualizarIntentosYEstadoAsync(int idUsuario, int intentosFallidos, string? codigoEstado = null);
    } 
}
