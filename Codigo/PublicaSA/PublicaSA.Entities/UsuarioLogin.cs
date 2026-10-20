using System;
using System.Collections.Generic;
using System.Text;

namespace PublicaSA.Entities
{
    public class UsuarioLogin
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty; // cifrada
        public string CodigoEstado { get; set; } = string.Empty;
        public int IntentosFallidos { get; set; }
    }
}
