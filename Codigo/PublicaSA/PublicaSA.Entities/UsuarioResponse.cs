using System;
using System.Collections.Generic;
using System.Text;

namespace PublicaSA.Entities
{
    public class UsuarioResponse
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty; 
    }
}
