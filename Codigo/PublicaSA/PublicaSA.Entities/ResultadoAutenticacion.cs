using System;
using System.Collections.Generic;
using System.Text;

namespace PublicaSA.Entities
{
    public class ResultadoAutenticacion
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public int? IdUsuario { get; set; }
        public string? NombreUsuario { get; set; }
        public string? NombreCompleto { get; set; }
        public string? CodigoEstado { get; set; } 
    }
}
