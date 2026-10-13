using System.Collections.Generic;
using System.Threading.Tasks;
using PublicaSA.Services.Interfaces;
using PublicaSA.Repository.Interfaces;
using PublicaSA.Entities;

namespace PublicaSA.Services.Services
{
    public class ReporteTareaService : IReporteTareaService
    {
        private readonly IReporteTareaRepository _reporteTareaRepository;

        public ReporteTareaService(IReporteTareaRepository reporteTareaRepository)
        {
            _reporteTareaRepository = reporteTareaRepository;
        }

        public async Task<IEnumerable<Usuario>> ObtenerUsuariosAsync()
        {
            return await _reporteTareaRepository.GetUsuariosAsync();
        }

        public async Task<IEnumerable<ReporteTarea>> ObtenerTareasPorUsuarioAsync(int idUsuario, int mes, int anio)
        {
            return await _reporteTareaRepository.GetTareasPorUsuarioAsync(idUsuario, mes, anio);
        }

        public async Task<IEnumerable<Solicitud>> ObtenerSolicitudesAsync()
        {
            return await _reporteTareaRepository.GetSolicitudesAsync();
        }

        public async Task<IEnumerable<ReporteTarea>> ObtenerTareasPorSolicitudAsync(int idSolicitud, int mes, int anio)
        {
            return await _reporteTareaRepository.GetTareasPorSolicitudAsync(idSolicitud, mes, anio);
        }
    }
}
