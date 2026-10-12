using System;
using System.Collections.Generic;
using System.Text;
using PublicaSA.Entities; 

namespace PublicaSA.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<(IEnumerable<UsuarioResponse> ListaUsuarios, int TotalRegistros)> ListarUsuarios(int pagina, int tamanoPagina);
    }
}
