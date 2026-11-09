using System.Collections.Generic;
using System.ServiceModel;
using Models;

namespace PublicaSA_WebService
{
    [ServiceContract]
    public interface IService1
    {
        // HU EXT2: Lista de solicitudes publicadas para proveedores
        [OperationContract]
        List<SolicitudPublicadaDto> ObtenerSolicitudesPublicadas();

        // HU EXT3: Detalle de una solicitud seleccionada
        [OperationContract]
        SolicitudDetalleDto ObtenerDetalleSolicitud(int idSolicitud);
    }
}
