using PublicaSA.Entities;

namespace PublicaSA.Services.Interfaces
{
    public interface IReporteMontosService
    {
        // Genera las filas y sumas del reporte y registra quién lo consultó.
        Task<ReporteMontosMensual> GenerarAsync(
            int mes,
            int anio,
            string usuarioActual);
    }
}