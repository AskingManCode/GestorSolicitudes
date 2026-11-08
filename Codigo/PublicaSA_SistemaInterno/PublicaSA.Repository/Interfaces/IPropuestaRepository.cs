using System.Collections.Generic;
using System.Threading.Tasks;
using PublicaSA.Entities;

namespace PublicaSA.Repository.Interfaces
{
    public interface IPropuestaRepository
    {
        Task<IEnumerable<Propuesta>> ListarPorSolicitudAsync(int idSolicitud);
        Task<(Propuesta? Propuesta, IEnumerable<DesglosePropuesta> Desgloses)> ObtenerDetalleAsync(int idPropuesta);
        Task<Propuesta?> AprobarAsync(int idPropuesta, int idSolicitud);
    }
}
