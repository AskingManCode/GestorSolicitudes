using System;

namespace PublicaSA.Entities
{
    public class SolicitudPublicacionRequest
    {
        public int IdSolicitud { get; set; }
        public bool VisibleExterno { get; set; }
        public DateTime? FechaVencimientoPublicacion { get; set; }
    }
}
