using System;
using System.Collections.Generic;
using System.Text;

namespace PublicaSA.Entities
{
    public class Usuario // Adaptar o borrar clase
    {
        public int IdUsuario { get; set; }
        public String NombreUsuario { get; set; } = String.Empty;
        public String NombreCompleto { get; set; } = String.Empty;
        public String Correo { get; set; } = String.Empty;
        public String Contrasena {  get; set; } = String.Empty;
        public String Estado {  get; set; } = "Activo";
        public int IntentosFallidos { get; set; } = 0;
        public DateTime FechaCreacion {  get; set; } = DateTime.Now;


    }
}
