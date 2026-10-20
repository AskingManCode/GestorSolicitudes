using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PublicaSA.Services.Interfaces;

namespace PublicaSA.Website.Pages.Login
{
    public class IndexModel : PageModel
    {
        private readonly IAutenticacionService _autenticacionService;

        public IndexModel(IAutenticacionService autenticacionService)
        {
            _autenticacionService = autenticacionService;
        }

        [BindProperty]
        public string Usuario { get; set; } = string.Empty;

        [BindProperty]
        public string Contrasena { get; set; } = string.Empty;

        public bool MostrarError { get; set; }
        public string MensajeError { get; set; } = "Usuario y/o contraseña incorrectos.";

        public void OnGet(string? mensaje = null)
        {
            // Logout automático al entrar a la página de login
            HttpContext.Session.Clear();

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
                return RedirectToPage(new { mensaje = "Usuario y/o contraseña incorrectos." });
            }

            var resultado = await _autenticacionService.AutenticarAsync(Usuario.Trim(), Contrasena);

            if (!resultado.Exitoso)
            {
                var mensaje = string.IsNullOrWhiteSpace(resultado.Mensaje)
                    ? "Usuario y/o contraseña incorrectos."
                    : resultado.Mensaje;

                return RedirectToPage(new { mensaje });
            }

            // Iniciar sesión
            HttpContext.Session.SetInt32("IdUsuario", resultado.IdUsuario ?? 0);
            HttpContext.Session.SetString("NombreUsuario", resultado.NombreUsuario ?? string.Empty);
            HttpContext.Session.SetString("NombreCompleto", resultado.NombreCompleto ?? string.Empty);

            return RedirectToPage("/Index");
        }
    }
}