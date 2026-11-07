using System.Collections.Generic;
using System.Threading.Tasks;
using PublicaSA.Entities;

namespace PublicaSA.Repository.Interfaces
{
    public interface IRepresentanteRepository
    {
        Task<IEnumerable<Representante>> GetAllAsync();
        Task<Representante?> GetByIdAsync(int id);
        Task<(IEnumerable<Representante> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
        Task<int> InsertAsync(Representante representante);
        Task<bool> UpdateAsync(Representante representante);
        Task<bool> DeleteAsync(int id);
        Task<bool> HasRelatedDataAsync(int id);
    }
}
