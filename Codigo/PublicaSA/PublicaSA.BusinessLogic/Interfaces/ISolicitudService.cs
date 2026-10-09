using System.Collections.Generic;
using System.Threading.Tasks;
using PublicaSA.BusinessLogic.Models;
using PublicaSA.Entities;

namespace PublicaSA.BusinessLogic.Interfaces
{
    public interface ISolicitudService
    {
        Task<(IEnumerable<Solicitud> Items, int TotalCount)> ObtenerPaginadoAsync(int pagina, int registrosPorPagina);
        Task<Solicitud?> ObtenerPorIdAsync(int id);
        Task<ResultadoOperacion> CrearAsync(Solicitud solicitud, string usuario);
        Task<ResultadoOperacion> ActualizarAsync(Solicitud solicitud, string usuario);
        Task<ResultadoOperacion> EliminarAsync(int id, string usuario);
        Task<IEnumerable<Representante>> ObtenerRepresentantesAsync();
        Task<IEnumerable<EstadoSolicitud>> ObtenerEstadosAsync();
    }
}
