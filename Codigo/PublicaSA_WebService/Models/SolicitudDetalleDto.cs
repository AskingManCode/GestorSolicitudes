using System;
using System.Runtime.Serialization;

namespace Models
{
    // Modelo para el detalle de la solicitud (EXT3)
    [DataContract]
    public class SolicitudDetalleDto
    {
        [DataMember]
        public int IdSolicitud { get; set; }

        [DataMember]
        public string ConsecutivoOficio { get; set; }

        [DataMember]
        public string DocumentoRespuesta { get; set; }

        [DataMember]
        public string DocumentoInicio { get; set; }

        [DataMember]
        public string Titulo { get; set; }

        [DataMember]
        public string Descripcion { get; set; }

        [DataMember]
        public string NombreRepresentante { get; set; }

        [DataMember]
        public string Observaciones { get; set; }

        [DataMember]
        public string NombreEstado { get; set; }

        [DataMember]
        public DateTime FechaIngreso { get; set; }

        [DataMember]
        public DateTime? FechaRespuesta { get; set; }

        [DataMember]
        public DateTime? FechaInicio { get; set; }

        [DataMember]
        public DateTime? FechaVencimientoPublicacion { get; set; }
    }
}
