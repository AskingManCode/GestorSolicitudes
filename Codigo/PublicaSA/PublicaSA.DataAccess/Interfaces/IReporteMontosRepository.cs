using PublicaSA.Entities;

namespace PublicaSA.DataAccess.Interfaces
{
    public interface IReporteMontosRepository
    {
        // Consulta los importes de las solicitudes del mes y año indicados.
        Task<IEnumerable<MontoSolicitudMensual>> ListarPorMesAsync(
            int mes,
            int anio);
    }
}