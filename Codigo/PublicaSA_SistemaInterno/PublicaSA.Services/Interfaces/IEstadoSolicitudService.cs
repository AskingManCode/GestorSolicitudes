using System.Collections.Generic;
using System.Threading.Tasks;
using PublicaSA.Entities;
using PublicaSA.Services.Models;

namespace PublicaSA.Services.Interfaces
{
    // HU SOL4: administración de estados de solicitudes.
    public interface IEstadoSolicitudService
    {
        Task<(IEnumerable<EstadoSolicitud> Items, int TotalCount)> ObtenerPaginadoAsync(int pagina, int registrosPorPagina, string usuario);
        Task<EstadoSolicitud?> ObtenerPorCodigoAsync(string codigo);
        Task<ResultadoOperacion> CrearAsync(EstadoSolicitud estado, string usuario);
        Task<ResultadoOperacion> ActualizarAsync(EstadoSolicitud estado, string usuario);
        Task<ResultadoOperacion> EliminarAsync(string codigo, string usuario);
        bool EsEstadoPorDefecto(string codigo);
    }
}
