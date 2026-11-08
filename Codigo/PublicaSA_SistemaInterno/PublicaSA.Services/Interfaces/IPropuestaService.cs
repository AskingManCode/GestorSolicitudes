using System.Collections.Generic;
using System.Threading.Tasks;
using PublicaSA.Entities;
using PublicaSA.Services.Models;

namespace PublicaSA.Services.Interfaces
{
    public interface IPropuestaService
    {
        Task<IEnumerable<Propuesta>> ObtenerPorSolicitudAsync(int idSolicitud, string usuario = "admin");
        Task<(Propuesta? Propuesta, IEnumerable<DesglosePropuesta> Desgloses)> ObtenerDetalleAsync(int idPropuesta, string usuario = "admin");
        Task<ResultadoOperacion> AprobarAsync(int idPropuesta, int idSolicitud, string usuario = "admin");
    }
}
