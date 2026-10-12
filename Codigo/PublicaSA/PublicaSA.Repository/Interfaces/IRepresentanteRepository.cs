using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using PublicaSA.Entities;

namespace PublicaSA.Repository.Interfaces
{
    public interface IRepresentanteRepository
    {
        Task<IEnumerable<Representante>> GetAllAsync();
        Task<Representante?> GetByIdAsync(int id);
    }
}
