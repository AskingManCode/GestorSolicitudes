using System.Collections.Generic;
using System.ServiceModel;
using Models;

namespace PublicaSA_WebService
{
    [ServiceContract]
    public interface IService1
    {
        // HU EXT2: solicitudes publicadas para proveedores.
        [OperationContract]
        List<SolicitudPublicadaDto> ObtenerSolicitudesPublicadas();

        // HU EXT3: detalle de la solicitud seleccionada.
        [OperationContract]
        SolicitudDetalleDto ObtenerDetalleSolicitud(int idSolicitud);

        // HU EXT4: registra la participación y sus desgloses.
        [OperationContract]
        ResultadoRegistroPropuesta RegistrarPropuesta(
            RegistroPropuestaRequest registro);
    }
}
