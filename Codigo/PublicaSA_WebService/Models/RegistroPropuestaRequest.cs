using System.Collections.Generic;

namespace Models
{
    public class RegistroPropuestaRequest
    {
        // Solicitud en la que desea participar el proveedor.
        public int IdSolicitud { get; set; }

        // Incluye la cédula y los datos para registrar un proveedor nuevo.
        public Proveedor Proveedor { get; set; }

        // Contiene las líneas ingresadas manualmente o desde Excel.
        public List<DesglosePropuesta> Desgloses { get; set; }
            = new List<DesglosePropuesta>();
    }
}