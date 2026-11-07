using PublicaSA.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PublicaSA.Services.Interfaces
{
    public interface IEstadosUsuarioService
    {
        Task<IEnumerable<EstadoUsuario>> ListarEstadosUsuario();
    }
}
