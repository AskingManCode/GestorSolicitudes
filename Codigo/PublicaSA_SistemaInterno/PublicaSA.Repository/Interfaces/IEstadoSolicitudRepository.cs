using System.Collections.Generic;
using System.Threading.Tasks;
using PublicaSA.Entities;

namespace PublicaSA.Repository.Interfaces
{
    public interface IEstadoSolicitudRepository
    {
        Task<IEnumerable<EstadoSolicitud>> GetAllAsync();
        Task<EstadoSolicitud?> GetByCodigoAsync(string codigo);
        Task<EstadoSolicitud?> GetByNombreAsync(string nombre);
    }
}