using System.Collections.Generic;
using System.Threading.Tasks;
using PublicaSA.Entities;

namespace PublicaSA.Services.Interfaces
{
    public interface IReporteTareaService
    {
        Task<IEnumerable<Usuario>> ObtenerUsuariosAsync();
        Task<IEnumerable<ReporteTarea>> ObtenerTareasPorUsuarioAsync(int idUsuario, int mes, int anio);
        Task<IEnumerable<Solicitud>> ObtenerSolicitudesAsync();
        Task<IEnumerable<ReporteTarea>> ObtenerTareasPorSolicitudAsync(int idSolicitud, int mes, int anio);
    }
}
