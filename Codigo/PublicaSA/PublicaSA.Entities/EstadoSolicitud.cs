using System;

namespace PublicaSA.Entities
{
    public class EstadoSolicitud
    {
        public string CodigoEstado { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
