using System;
using System.Collections.Generic;

namespace Models
{
    public class Propuesta
    {
        public int IdPropuesta { get; set; }
        public int IdSolicitud { get; set; }
        public int IdProveedor { get; set; }

        // Las propuestas se registran sin aprobar.
        public bool Aprobada { get; set; } = false;

        public DateTime FechaPropuesta { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public bool Activo { get; set; } = true;

        // Reúne las líneas ingresadas en pantalla o cargadas desde Excel.
        public List<DesglosePropuesta> Desgloses { get; set; }
            = new List<DesglosePropuesta>();
    }
}