using System.Threading.Tasks;

namespace PublicaSA.Services.Interfaces
{
    public interface IBitacoraService
    {
        Task RegistrarAccionAsync(string usuario, string accion, object detalle);
        Task RegistrarErrorAsync(string usuario, string accion, string mensajeError);
    }
}
