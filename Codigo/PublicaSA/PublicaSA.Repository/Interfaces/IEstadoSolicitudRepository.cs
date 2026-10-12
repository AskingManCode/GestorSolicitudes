using PublicaSA.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PublicaSA.Repository.Interfaces
{
    public interface IEstadoSolicitudRepository // Adaptar o borrar interface
    {
        Task<IEnumerable<EstadoSolicitud>> GetAllAsync();
        Task<EstadoSolicitud?> GetByIdAsync(int id);
        Task<EstadoSolicitud?> GetByNombreAsync(string nombre);
    }
}