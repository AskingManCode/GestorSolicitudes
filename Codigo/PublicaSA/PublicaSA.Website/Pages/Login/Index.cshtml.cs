using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PublicaSA.Services.Interfaces;

namespace PublicaSA.Website.Pages.Login
{
    public class IndexModel : PageModel
    {
        private readonly IAutenticacionService _autenticacionService;
        [BindProperty]
        public string Usuario { get; set; } = string.Empty;
        [BindProperty]
        public string Contrasena { get; set; } = string.Empty;
        public bool MostrarError { get; set; }
        public string MensajeError { get; set; } = "Usuario y/o contraseña incorrectos.";

        public IndexModel(IAutenticacionService autenticacionService)
        {
            _autenticacionService = autenticacionService;
        }

        public void OnGet(string? mensaje = null)
        {
            // Mensaje cuando se redirige por falta de sesión
            if (!string.IsNullOrWhiteSpace(mensaje))
            {
                MostrarError = true;
                MensajeError = mensaje;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Usuario) || string.IsNullOrWhiteSpace(Contrasena))
            {
                MostrarError = true;
                MensajeError = "Usuario y/o contraseña incorrectos.";
                return Page();
            }

            var resultado = await _autenticacionService.AutenticarAsync(Usuario.Trim(), Contrasena);

            if (!resultado.Exitoso)
            {
                MostrarError = true;
                MensajeError = string.IsNullOrWhiteSpace(resultado.Mensaje)
                    ? "Usuario y/o contraseña incorrectos."
                    : resultado.Mensaje;
                return Page();
            }

            // Iniciar sesión con HttpContext.Session
            HttpContext.Session.SetInt32("IdUsuario", resultado.IdUsuario ?? 0);
            HttpContext.Session.SetString("NombreUsuario", resultado.NombreUsuario ?? string.Empty);
            HttpContext.Session.SetString("NombreCompleto", resultado.NombreCompleto ?? string.Empty);

            return RedirectToPage("/Index");
        }
    }
}