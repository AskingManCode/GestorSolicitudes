using System.Collections.Generic;
using System.Threading.Tasks;
using PublicaSA.Entities;

namespace PublicaSA.DataAccess.Interfaces
{
    public interface IEstadoSolicitudRepository
    {
        Task<IEnumerable<EstadoSolicitud>> GetAllAsync();
        Task<EstadoSolicitud?> GetByIdAsync(int id);
        Task<EstadoSolicitud?> GetByNombreAsync(string nombre);
    }
}
