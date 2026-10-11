using PublicaSA.Entities;

namespace PublicaSA.DataAccess.Interfaces
{
    public interface ISolicitudVencimientoRepository
    {
        // Obtiene las solicitudes sin respuesta que aún no están vencidas.
        Task<IEnumerable<SolicitudPendienteVencimiento>>
            ListarPendientesAsync();

        // Comprueba que la fecha de ingreso no cambió antes de actualizar.
        Task<bool> MarcarVencidaAsync(
            int idSolicitud,
            DateTime fechaIngresoEsperada,
            DateTime fechaVencimiento,
            DateTime fechaActual);
    }
}