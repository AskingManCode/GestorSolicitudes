using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PublicaSA.Entities;
using PublicaSA.Services.Interfaces;

namespace PublicaSA.Website.Pages.Usuarios
{
    public class EditModel : PageModelAutenticado
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IEstadosUsuarioService _estadosUsuarioService;

        public IEnumerable<EstadoUsuario> ListaEstados { get; set; } = new List<EstadoUsuario>();

        [BindProperty]
        public UsuarioEditRequest Usuario { get; set; } = new();

        public EditModel(
            IUsuarioService usuarioService,
            IEstadosUsuarioService estadosUsuarioService)
        {
            _usuarioService = usuarioService;
            _estadosUsuarioService = estadosUsuarioService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            ListaEstados = await _estadosUsuarioService.ListarEstadosUsuario();

            var usuario = await _usuarioService.ObtenerPorId(id);
            if (usuario == null)
            {
                return RedirectToPage("./Index", new { success = true, message = "Usuario no encontrado.", tipo = "warning", title = "Advertencia" });
            }

            Usuario = new UsuarioEditRequest
            {
                IdUsuario = usuario.IdUsuario,
                NombreCompleto = usuario.NombreCompleto,
                Correo = usuario.Correo,
                NombreUsuario = usuario.NombreUsuario,
                CodigoEstado = usuario.CodigoEstado
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ListaEstados = await _estadosUsuarioService.ListarEstadosUsuario();

            try
            {
                await _usuarioService.Editar(Usuario);
                return RedirectToPage("./Index", new { success = true, message = "Usuario actualizado correctamente.", tipo = "success", title = "Éxito" });
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