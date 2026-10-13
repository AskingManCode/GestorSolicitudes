using PublicaSA.Entities;

namespace PublicaSA.Repository.Interfaces
{
    public interface IReporteMontosRepository
    {
        // Consulta los importes de las solicitudes del período seleccionado.
        Task<IEnumerable<MontoSolicitudMensual>> ListarPorMesAsync(
            int mes,
            int anio);
    }
}