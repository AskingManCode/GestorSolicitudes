using PublicaSA.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PublicaSA.Repository.Interfaces
{
    public interface IAutenticacionRepository
    {
        Task<UsuarioLogin?> ObtenerPorNombreUsuarioAsync(string nombreUsuario);
        Task ActualizarIntentosYEstadoAsync(int idUsuario, int intentosFallidos, string? codigoEstado = null);
    }
}
