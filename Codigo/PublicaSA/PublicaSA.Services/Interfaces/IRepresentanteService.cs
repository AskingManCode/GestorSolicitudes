using System.Collections.Generic;
using System.Threading.Tasks;
using PublicaSA.Services.Models;
using PublicaSA.Entities;

namespace PublicaSA.Services.Interfaces
{
    public interface IRepresentanteService
    {
        Task<(IEnumerable<Representante> Items, int TotalCount)> ObtenerPaginadoAsync(int pagina, int registrosPorPagina, string usuario);
        Task<Representante?> ObtenerPorIdAsync(int id);
        Task<ResultadoOperacion> CrearAsync(Representante representante, string usuario);
        Task<ResultadoOperacion> ActualizarAsync(Representante representante, string usuario);
        Task<ResultadoOperacion> EliminarAsync(int id, string usuario);
    }
}
