using System.Collections.Generic;
using DataAccess.Repository;
using Models;

namespace BusinessLogic.Services
{
    public class SolicitudExternaService
    {
        private readonly SolicitudExternaRepository _repository;

        public SolicitudExternaService()
        {
            _repository = new SolicitudExternaRepository();
        }

        // Obtiene las solicitudes activas para mostrar en el listado externo (EXT2)
        public List<SolicitudPublicadaDto> ObtenerSolicitudesPublicadas()
        {
            return _repository.ObtenerSolicitudesPublicadas();
        }

        // Obtiene la información completa de una solicitud (EXT3)
        public SolicitudDetalleDto ObtenerDetalleSolicitud(int idSolicitud)
        {
            if (idSolicitud <= 0)
            {
                return null;
            }

            return _repository.ObtenerDetalleSolicitud(idSolicitud);
        }
    }
}
