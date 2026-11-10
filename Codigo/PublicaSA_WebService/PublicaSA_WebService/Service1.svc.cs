using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using BusinessLogic.Services;
using DataAccess.Infrastructure;
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

        public List<SolicitudPublicadaDto> ObtenerSolicitudesPublicadas()
        {
            return _solicitudService.ObtenerSolicitudesPublicadas();
        }

        public SolicitudDetalleDto ObtenerDetalleSolicitud(int idSolicitud)
        {
            return _solicitudService.ObtenerDetalleSolicitud(idSolicitud);
        }

        public ResultadoRegistroPropuesta RegistrarPropuesta(
            RegistroPropuestaRequest registro)
        {
            // La conexión continúa fuera del repositorio.
            var configuracion =
                ConfigurationManager.ConnectionStrings["DefaultConnection"];

            if (configuracion == null ||
                string.IsNullOrWhiteSpace(configuracion.ConnectionString))
            {
                Trace.TraceError(
                    "No está configurada la conexión DefaultConnection.");

                return new ResultadoRegistroPropuesta
                {
                    Exitoso = false,
                    Mensaje = "El servicio no está disponible en este momento."
                };
            }

            var conexionBD = new ConexionBD(configuracion.ConnectionString);
            var servicio = new RegistroPropuestaService(conexionBD);
            return servicio.Registrar(registro);
        }
    }
}
