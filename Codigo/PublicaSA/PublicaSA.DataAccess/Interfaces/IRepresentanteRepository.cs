using System.Collections.Generic;
using System.Threading.Tasks;
using PublicaSA.Entities;

namespace PublicaSA.DataAccess.Interfaces
{
    public interface IRepresentanteRepository
    {
        Task<IEnumerable<Representante>> GetAllAsync();
        Task<Representante?> GetByIdAsync(int id);
    }
}
