using System.Collections.Generic;
using System.Threading.Tasks;
using PublicaSA.Entities;

namespace PublicaSA.DataAccess.Interfaces
{
    public interface ISolicitudRepository
    {
        Task<(IEnumerable<Solicitud> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
        Task<Solicitud?> GetByIdAsync(int id);
        Task<Solicitud?> GetByConsecutivoAsync(string consecutivo);
        Task<int> InsertAsync(Solicitud solicitud);
        Task<bool> UpdateAsync(Solicitud solicitud);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasRelatedDataAsync(int id);
    }
}
