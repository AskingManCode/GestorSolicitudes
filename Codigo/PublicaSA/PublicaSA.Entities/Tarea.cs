using System;

namespace PublicaSA.Entities
{
    public class Tarea
    {
        public int IdTarea { get; set; }
        public int IdSolicitud { get; set; }
        public int IdUsuario { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.Today;
        public decimal HorasInvertidas { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Propiedad informativa
        public string? NombreUsuario { get; set; }
    }
}
