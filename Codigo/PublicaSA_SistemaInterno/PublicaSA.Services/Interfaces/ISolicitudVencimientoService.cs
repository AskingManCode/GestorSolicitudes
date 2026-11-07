namespace PublicaSA.Services.Interfaces
{
    public interface ISolicitudVencimientoService
    {
        // Revisa las solicitudes y devuelve cuántas cambiaron a Vencida.
        Task<int> ProcesarAsync(DateTime fechaActual);
    }
}