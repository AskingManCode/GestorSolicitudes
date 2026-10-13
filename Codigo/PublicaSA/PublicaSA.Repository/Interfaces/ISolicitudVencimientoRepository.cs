using PublicaSA.Entities;

namespace PublicaSA.Repository.Interfaces
{
    public interface ISolicitudVencimientoRepository
    {
        // Obtiene las solicitudes sin respuesta que aún no están vencidas.
        Task<IEnumerable<Solicitud>> ListarPendientesAsync();

        // Revisa nuevamente la solicitud antes de cambiar el estado y auditar.
        Task<bool> MarcarVencidaAsync(
            int idSolicitud,
            DateTime fechaActual);
    }
}