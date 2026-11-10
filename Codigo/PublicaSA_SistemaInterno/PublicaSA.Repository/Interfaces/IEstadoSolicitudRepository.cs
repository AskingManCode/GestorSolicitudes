using System.Collections.Generic;
using System.Threading.Tasks;
using PublicaSA.Entities;

namespace PublicaSA.Repository.Interfaces
{
    public interface IEstadoSolicitudRepository
    {
        // Usado por la lista de selección de Solicitudes (solo estados activos).
        Task<IEnumerable<EstadoSolicitud>> GetAllAsync();
        Task<EstadoSolicitud?> GetByCodigoAsync(string codigo);
        Task<EstadoSolicitud?> GetByNombreAsync(string nombre);

        // HU SOL4: mantenimiento de estados de solicitudes.
        Task<(IEnumerable<EstadoSolicitud> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
        Task<bool> InsertAsync(EstadoSolicitud estado);
        Task<bool> UpdateAsync(EstadoSolicitud estado);
        Task<bool> DeleteAsync(string codigo);
        Task<bool> HasRelatedDataAsync(string codigo);
    }
}
