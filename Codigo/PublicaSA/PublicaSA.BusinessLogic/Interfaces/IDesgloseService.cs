using System.Collections.Generic;
using System.Threading.Tasks;
using PublicaSA.BusinessLogic.Models;
using PublicaSA.Entities;

namespace PublicaSA.BusinessLogic.Interfaces
{
    public interface IDesgloseService
    {
        Task<(IEnumerable<DesgloseSolicitud> Items, int TotalCount)> ObtenerPaginadoPorSolicitudAsync(int idSolicitud, int pagina, int registrosPorPagina);
        Task<IEnumerable<DesgloseSolicitud>> ObtenerPorSolicitudIdAsync(int idSolicitud);
        Task<DesgloseSolicitud?> ObtenerPorIdAsync(int id);
        Task<ResultadoOperacion> CrearAsync(DesgloseSolicitud desglose, string usuario);
        Task<ResultadoOperacion> ActualizarAsync(DesgloseSolicitud desglose, string usuario);
        Task<ResultadoOperacion> EliminarAsync(int id, string usuario);
    }
}
