using System.Threading.Tasks;

namespace PublicaSA.BusinessLogic.Interfaces
{
    public interface IBitacoraService
    {
        Task RegistrarAccionAsync(string usuario, string accion, object detalle);
        Task RegistrarErrorAsync(string usuario, string accion, string mensajeError);
    }
}
