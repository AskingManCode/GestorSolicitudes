using System.Runtime.Serialization;

namespace Models
{
    // HU EXT6: solicitud en la que participó un proveedor.
    [DataContract]
    public class ParticipacionProveedorDto
    {
        [DataMember]
        public int IdSolicitud { get; set; }

        [DataMember]
        public string ConsecutivoOficio { get; set; }

        [DataMember]
        public string Titulo { get; set; }
    }
}
