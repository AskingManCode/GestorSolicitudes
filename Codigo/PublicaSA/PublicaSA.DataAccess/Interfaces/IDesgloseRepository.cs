using System.Collections.Generic;
using System.Threading.Tasks;
using PublicaSA.Entities;

namespace PublicaSA.DataAccess.Interfaces
{
    public interface IDesgloseRepository
    {
        Task<(IEnumerable<DesgloseSolicitud> Items, int TotalCount)> GetPagedBySolicitudAsync(int idSolicitud, int pageNumber, int pageSize);
        Task<IEnumerable<DesgloseSolicitud>> GetBySolicitudIdAsync(int idSolicitud);
        Task<DesgloseSolicitud?> GetByIdAsync(int id);
        Task<int> InsertAsync(DesgloseSolicitud desglose);
        Task<bool> UpdateAsync(DesgloseSolicitud desglose);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasRelatedDataAsync(int id);
    }
}
