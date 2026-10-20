using PublicaSA.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PublicaSA.Services.Interfaces
{
    public interface IAutenticacionService
    {
        Task<ResultadoAutenticacion> AutenticarAsync(string nombreUsuario, string contrasena);
    }
}
