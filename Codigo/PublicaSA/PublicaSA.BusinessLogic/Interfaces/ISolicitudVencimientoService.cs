namespace PublicaSA.BusinessLogic.Interfaces
{
    public interface ISolicitudVencimientoService
    {
        // Revisa los vencimientos y devuelve cuántas solicitudes cambió.
        Task<int> ProcesarAsync(DateTime fechaActual);
    }
}