using System;

namespace PublicaSA.Entities
{
    public class Propuesta
    {
        public int IdPropuesta { get; set; }
        public int IdSolicitud { get; set; }
        public int IdProveedor { get; set; }
        public bool Aprobada { get; set; } = false;
        public DateTime FechaPropuesta { get; set; } = DateTime.Now;
        public DateTime? FechaAprobacion { get; set; }
        public bool Activo { get; set; } = true;

        // Datos relacionados de la solicitud y del proveedor para visualización en pantalla
        public string ConsecutivoOficio { get; set; } = string.Empty;
        public string TituloSolicitud { get; set; } = string.Empty;
        public string CedulaJuridica { get; set; } = string.Empty;
        public string NombreProveedor { get; set; } = string.Empty;
        public string? NombreRepresentante { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }

        // Indica si la solicitud asociada ya cuenta con una propuesta aprobada
        public bool SolicitudTienePropuestaAprobada { get; set; }
    }
}
