using PublicaSA.BusinessLogic.Helpers;
using PublicaSA.BusinessLogic.Interfaces;
using PublicaSA.DataAccess.Interfaces;
using System.Text.Json;

namespace PublicaSA.BusinessLogic.Services
{
    public class SolicitudVencimientoService
        : ISolicitudVencimientoService
    {
        private readonly ISolicitudVencimientoRepository _solicitudes;
        private readonly IBitacoraRepository _bitacoras;

        public SolicitudVencimientoService(
            ISolicitudVencimientoRepository solicitudes,
            IBitacoraRepository bitacoras)
        {
            _solicitudes = solicitudes;
            _bitacoras = bitacoras;
        }

        public async Task<int> ProcesarAsync(DateTime fechaActual)
        {
            int cantidadActualizada = 0;

            try
            {
                var pendientes =
                    await _solicitudes.ListarPendientesAsync();

                foreach (var solicitud in pendientes)
                {
                    // Calcula cinco días hábiles desde la fecha de ingreso.
                    DateTime fechaVencimiento =
                        CalculadoraVencimiento.CalcularFechaVencimiento(
                            solicitud.FechaIngreso);

                    if (fechaActual.Date <= fechaVencimiento.Date)
                    {
                        continue;
                    }

                    bool actualizada =
                        await _solicitudes.MarcarVencidaAsync(
                            solicitud.IdSolicitud,
                            solicitud.FechaIngreso,
                            fechaVencimiento,
                            fechaActual.Date);

                    if (actualizada)
                    {
                        cantidadActualizada++;
                    }
                }

                return cantidadActualizada;
            }
            catch (Exception error)
            {
                try
                {
                    // Identifica el error como una acción del proceso automático.
                    await _bitacoras.RegistrarAsync(
                        null,
                        "ERROR",
                        "Error en el proceso automático de vencimiento. "
                        + JsonSerializer.Serialize(new
                        {
                            FechaActual = fechaActual.Date,
                            CantidadActualizada = cantidadActualizada,
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