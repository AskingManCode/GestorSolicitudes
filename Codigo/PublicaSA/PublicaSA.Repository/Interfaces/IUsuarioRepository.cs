using System;
using System.Collections.Generic;
using System.Text;
using PublicaSA.Entities;

namespace PublicaSA.Repository.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<(IEnumerable<UsuarioResponse> ListaUsuarios, int TotalRegistros)> ListarUsuarios(int pagina, int tamanoPagina); 
    } 
}
