using PublicaSA.BusinessLogic.Interfaces;
using PublicaSA.DataAccess.Interfaces;
using PublicaSA.Entities;
using System.Text.Json;

namespace PublicaSA.BusinessLogic.Services
{
    public class ReporteMontosService : IReporteMontosService
    {
        private readonly IReporteMontosRepository _reportes;
        private readonly IBitacoraRepository _bitacoras;

        public ReporteMontosService(
            IReporteMontosRepository reportes,
            IBitacoraRepository bitacoras)
        {
            _reportes = reportes;
            _bitacoras = bitacoras;
        }

        public async Task<ReporteMontosMensual> GenerarAsync(
            int mes,
            int anio,
            int idUsuarioActual)
        {
            if (idUsuarioActual <= 0)
            {
                throw new ArgumentException(
                    "Se requiere un usuario autenticado.",
                    nameof(idUsuarioActual));
            }

            try
            {
                var solicitudes = (
                    await _reportes.ListarPorMesAsync(mes, anio)
                ).ToList();

                // Calcula las tres sumas solicitadas en SOL7.
                var reporte = new ReporteMontosMensual
                {
                    Solicitudes = solicitudes,
                    TotalMonto = solicitudes.Sum(fila => fila.Monto),
                    TotalIva = solicitudes.Sum(fila => fila.Iva),
                    TotalGeneral = solicitudes.Sum(fila => fila.Total)
                };

                await _bitacoras.RegistrarAsync(
                    idUsuarioActual,
                    "CONSULTA",
                    "El usuario consulta reporte de montos por mes");

                return reporte;
            }
            catch (Exception error)
            {
                try
                {
                    await _bitacoras.RegistrarAsync(
                        idUsuarioActual,
                        "ERROR",
                        "Error al generar el reporte de montos. "
                        + JsonSerializer.Serialize(new
                        {
                            Mes = mes,
                            Anio = anio,
                            TipoError = error.GetType().Name
                        }));
                }
                catch
                {
                    // Conserva el error original si también falla la bitácora.
                }

                throw;
            }
        }
    }
}