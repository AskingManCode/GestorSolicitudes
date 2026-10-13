using System;
using System.Collections.Generic;
using System.Text;
using PublicaSA.Entities; 

namespace PublicaSA.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<(IEnumerable<UsuarioResponse> ListaUsuarios, int TotalPaginas, int PaginaActual)> ListarUsuarios(int pagina, int tamanoPagina);
        Task<UsuarioResponse> Crear(UsuarioRequest request);
        Task<UsuarioResponse> CambiarEstado(int idUsuario, string codigoEstado);
    }
}
