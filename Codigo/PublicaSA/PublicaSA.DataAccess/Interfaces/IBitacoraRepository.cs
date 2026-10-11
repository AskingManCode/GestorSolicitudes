namespace PublicaSA.DataAccess.Interfaces
{
    public interface IBitacoraRepository
    {
        Task RegistrarAsync(
            int? idUsuario,
            string tipoAccion,
            string descripcion);
    }
}