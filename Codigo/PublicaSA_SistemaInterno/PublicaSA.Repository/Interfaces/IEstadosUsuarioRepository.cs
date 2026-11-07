using PublicaSA.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PublicaSA.Repository.Interfaces
{
    public interface IEstadosUsuarioRepository
    {
        Task<IEnumerable<EstadoUsuario>> ListarEstadosUsuario();
    }
}
