using System.Collections.Generic;
using System.Threading.Tasks;
using PublicaSA.Entities;

namespace PublicaSA.Repository.Interfaces
{
    public interface IReporteTareaRepository
    {
        Task<IEnumerable<Usuario>> GetUsuariosAsync();
        Task<IEnumerable<ReporteTarea>> GetTareasPorUsuarioAsync(int idUsuario, int mes, int anio);
        Task<IEnumerable<Solicitud>> GetSolicitudesAsync();
        Task<IEnumerable<ReporteTarea>> GetTareasPorSolicitudAsync(int idSolicitud, int mes, int anio);
    }
}
