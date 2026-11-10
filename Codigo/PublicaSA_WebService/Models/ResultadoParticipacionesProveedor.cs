using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Models
{
    // HU EXT6: respuesta de la consulta de participaciones de un proveedor.
    [DataContract]
    public class ResultadoParticipacionesProveedor
    {
        // Indica si se encontraron solicitudes para el proveedor.
        [DataMember]
        public bool Exitoso { get; set; }

        // "Proveedor no existe.", "Datos no encontrados." o el error de formato.
        [DataMember]
        public string Mensaje { get; set; }

        // Solo tiene valor cuando el proveedor existe.
        [DataMember]
        public string NombreProveedor { get; set; }

        [DataMember]
        public List<ParticipacionProveedorDto> Participaciones { get; set; }
            = new List<ParticipacionProveedorDto>();
    }
}
