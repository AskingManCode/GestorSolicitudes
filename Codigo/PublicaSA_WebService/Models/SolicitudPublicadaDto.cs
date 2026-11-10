using System;
using System.Runtime.Serialization;

namespace Models
{
    // Modelo para la lista de solicitudes publicadas (EXT2)
    [DataContract]
    public class SolicitudPublicadaDto
    {
        [DataMember]
        public int IdSolicitud { get; set; }

        [DataMember]
        public string ConsecutivoOficio { get; set; }

        [DataMember]
        public string Titulo { get; set; }

        [DataMember]
        public string NombreRepresentante { get; set; }
    }
}
