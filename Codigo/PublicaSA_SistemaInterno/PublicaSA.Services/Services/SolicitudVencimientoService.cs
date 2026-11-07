using PublicaSA.Entities;
using PublicaSA.Repository.Interfaces;
using PublicaSA.Services.Interfaces;

namespace PublicaSA.Services.Services
{
    public class SolicitudVencimientoService
        : ISolicitudVencimientoService
    {
        private readonly ISolicitudVencimientoRepository _solicitudes;
        private readonly IBitacoraService _bitacoras;

        public SolicitudVencimientoService(
            ISolicitudVencimientoRepository solicitudes,
            IBitacoraService bitacoras)
        {
            _solicitudes = solicitudes;
            _bitacoras = bitacoras;
        }

        public async Task<int> ProcesarAsync(DateTime fechaActual)
        {
            try
            {
                var pendientes =
                    await _solicitudes.ListarPendientesAsync();

                int cantidadVencidas = 0;

                foreach (var solicitud in pendientes)
                {
                    DateTime fechaLimite =
                        Solicitud.CalcularQuintoDiaHabil(
                            solicitud.FechaIngreso);

                    if (fechaActual.Date <= fechaLimite)
                    {
                        continue;
                    }

                    // El repositorio vuelve a comprobar los datos bajo bloqueo.
                    bool actualizada =
                        await _solicitudes.MarcarVencidaAsync(
                            solicitud.IdSolicitud,
                            fechaActual.Date);

                    if (actualizada)
                    {
                        cantidadVencidas++;
                    }
                }

                return cantidadVencidas;
            }
            catch (Exception error)
            {
                try
                {
                    await _bitacoras.RegistrarErrorAsync(
                        "Sistema",
                        "vencimiento automático de solicitudes",
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