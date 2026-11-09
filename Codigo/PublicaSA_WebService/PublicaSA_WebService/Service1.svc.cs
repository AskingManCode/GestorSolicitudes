using System.Collections.Generic;
using BusinessLogic.Services;
using Models;

namespace PublicaSA_WebService
{
    public class Service1 : IService1
    {
        private readonly SolicitudExternaService _solicitudService;

        public Service1()
        {
            _solicitudService = new SolicitudExternaService();
        }

        // HU EXT2: Lista de solicitudes publicadas para proveedores
        public List<SolicitudPublicadaDto> ObtenerSolicitudesPublicadas()
        {
            return _solicitudService.ObtenerSolicitudesPublicadas();
        }

        // HU EXT3: Detalle de una solicitud seleccionada
        public SolicitudDetalleDto ObtenerDetalleSolicitud(int idSolicitud)
        {
            return _solicitudService.ObtenerDetalleSolicitud(idSolicitud);
        }
    }
}
