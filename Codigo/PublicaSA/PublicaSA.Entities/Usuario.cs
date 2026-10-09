using System;

namespace PublicaSA.Entities
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public string Estado { get; set; } = "Activo";
        public int IntentosFallidos { get; set; } = 0;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
