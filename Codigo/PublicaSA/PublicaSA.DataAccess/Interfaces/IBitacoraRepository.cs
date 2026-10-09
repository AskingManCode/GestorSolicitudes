using System.Collections.Generic;
using System.Threading.Tasks;
using PublicaSA.Entities;

namespace PublicaSA.DataAccess.Interfaces
{
    public interface IBitacoraRepository
    {
        Task<int> InsertAsync(Bitacora bitacora);
        Task<IEnumerable<Bitacora>> GetAllAsync();
    }
}
