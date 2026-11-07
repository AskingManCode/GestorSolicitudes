using PublicaSA.Entities;
using PublicaSA.Repository.Interfaces;
using PublicaSA.Services.Interfaces;

namespace PublicaSA.Services.Services
{
    public class ReporteMontosService : IReporteMontosService
    {
        private readonly IReporteMontosRepository _reportes;
        private readonly IBitacoraService _bitacoras;

        public ReporteMontosService(
            IReporteMontosRepository reportes,
            IBitacoraService bitacoras)
        {
            _reportes = reportes;
            _bitacoras = bitacoras;
        }

        public async Task<ReporteMontosMensual> GenerarAsync(
            int mes,
            int anio,
            string usuarioActual)
        {
            if (string.IsNullOrWhiteSpace(usuarioActual))
            {
                throw new ArgumentException(
                    "Se requiere identificar al usuario actual.",
                    nameof(usuarioActual));
            }

            try
            {
                if (mes < 1 || mes > 12)
                {
                    throw new ArgumentOutOfRangeException(nameof(mes));
                }

                if (anio < 2000 || anio > 2150)
                {
                    throw new ArgumentOutOfRangeException(nameof(anio));
                }

                var filas = await _reportes.ListarPorMesAsync(mes, anio);
                var solicitudes = filas.ToList();

                // Calcula las tres sumas sobre las mismas filas del reporte.
                var reporte = new ReporteMontosMensual
                {
                    Solicitudes = solicitudes,
                    SumaMonto = solicitudes.Sum(fila => fila.Monto),
                    SumaIva = solicitudes.Sum(fila => fila.Iva),
                    SumaTotal = solicitudes.Sum(fila => fila.Total)
                };

                await _bitacoras.RegistrarAccionAsync(
                    usuarioActual,
                    "CONSULTA",
                    "El usuario consulta reporte de montos mensuales");

                return reporte;
            }
            catch (Exception error)
            {
                try
                {
                    await _bitacoras.RegistrarErrorAsync(
                        usuarioActual,
                        "generar reporte de montos mensuales",
                        error.GetType().Name);
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