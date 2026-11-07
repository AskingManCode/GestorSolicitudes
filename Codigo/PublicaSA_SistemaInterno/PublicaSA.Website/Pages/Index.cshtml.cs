using PublicaSA.Services.Interfaces;

namespace PublicaSA.Website.Pages
{
    public class IndexModel : PageModelAutenticado
    {
        private readonly IBitacoraService _bitacoraService;

        public IndexModel(IBitacoraService bitacoraService)
        {
            _bitacoraService = bitacoraService;
        }

        public string NombreCompleto { get; private set; } = "Usuario";

        public async Task OnGetAsync()
        {
            string usuario = "Sistema";

            try
            {
                usuario = HttpContext.Session.GetString("NombreUsuario") ?? "Sistema";
                var nombreCompleto = HttpContext.Session.GetString("NombreCompleto");

                if (!string.IsNullOrWhiteSpace(nombreCompleto))
                {
                    NombreCompleto = nombreCompleto;
                }
            }
            catch (Exception ex)
            {
                await _bitacoraService.RegistrarErrorAsync(
                    usuario,
                    "Cargar pantalla de bienvenida",
                    ex.Message);
            }
        }
    }
}
