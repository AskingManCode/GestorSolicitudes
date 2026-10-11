using PublicaSA.Entities;

namespace PublicaSA.BusinessLogic.Interfaces
{
    public interface IReporteMontosService
    {
        // Genera el reporte e identifica al usuario que lo consulta.
        Task<ReporteMontosMensual> GenerarAsync(
            int mes,
            int anio,
            int idUsuarioActual);
    }
}