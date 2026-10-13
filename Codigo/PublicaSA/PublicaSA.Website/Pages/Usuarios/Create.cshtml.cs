using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PublicaSA.Entities;
using PublicaSA.Services.Interfaces;

namespace PublicaSA.Website.Pages.Usuarios
{
    public class CreateModel : PageModel
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IEstadosUsuarioService _estadosUsuarioService;

        public IEnumerable<EstadoUsuario> ListaEstados { get; set; } = new List<EstadoUsuario>();
        public string EstadoActual { get; set; } = "ACTIVO";

        [BindProperty]
        public UsuarioRequest Usuario { get; set; } = new();

        public CreateModel(
            IUsuarioService usuarioService,
            IEstadosUsuarioService estadosUsuarioService)
        {
            _usuarioService = usuarioService;
            _estadosUsuarioService = estadosUsuarioService;
        }

        public async Task OnGetAsync()
        {
            ListaEstados = await _estadosUsuarioService.ListarEstadosUsuario();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ListaEstados = await _estadosUsuarioService.ListarEstadosUsuario();

            try
            {
                await _usuarioService.Crear(Usuario);
                return RedirectToPage("./Index", new { success = true, message = "Usuario creado correctamente." });
            }
            catch (ValidationException ex)
            {
                var mensajes = string.Join("<br/>", ex.Errors.Select(e => e.ErrorMessage));
                // Validación => advertencia (amarillo)
                TempData["ModalMessage"] = mensajes;
                TempData["ModalType"] = "warning";
                TempData["ModalTitle"] = "Advertencia";
                return Page();
            }
            catch (Exception ex)
            {
                TempData["ModalMessage"] = ex.Message;
                TempData["ModalType"] = "error";
                TempData["ModalTitle"] = "Error";
                return Page();
            }
        }
    }
}