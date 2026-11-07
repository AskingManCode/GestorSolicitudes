using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PublicaSA.Entities;
using PublicaSA.Services.Interfaces;

namespace PublicaSA.Website.Pages.Usuarios
{
    public class IndexModel : PageModelAutenticado
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IEstadosUsuarioService _estadosUsuarioService;

        public IEnumerable<UsuarioResponse> ListaUsuarios { get; set; } = new List<UsuarioResponse>();
        public IEnumerable<EstadoUsuario> ListaEstados { get; set; } = new List<EstadoUsuario>();
        
        public int TotalPaginas { get; set; } = 1;
        public int PaginaActual { get; set; } = 1;
        public bool ShowAlert { get; set; } = false;
        public string? AlertMessage { get; set; }
        public string AlertType { get; set; } = "success"; // success | error | warning
        public string AlertTitle { get; set; } = "Mensaje";

        public IndexModel(IUsuarioService usuarioService, IEstadosUsuarioService estadosUsuarioService)
        {
            _usuarioService = usuarioService;
            _estadosUsuarioService = estadosUsuarioService;
        }

        public async Task OnGetAsync(int pagina = 1, bool success = false, string? message = null, string? tipo = null, string? title = null)
        {
            // Carga estados para los dropdowns
            ListaEstados = await _estadosUsuarioService.ListarEstadosUsuario();

            // Carga usuarios paginados
            var (listaUsuarios, totalPaginas, paginaActual) = await _usuarioService.ListarUsuarios(pagina, 10);
            ListaUsuarios = listaUsuarios;
            TotalPaginas = totalPaginas;
            PaginaActual = paginaActual;

            // Leer parámetros de alerta
            ShowAlert = success;
            AlertMessage = message;
            AlertType = string.IsNullOrWhiteSpace(tipo) ? "success" : tipo;
            AlertTitle = string.IsNullOrWhiteSpace(title) ? (AlertType == "success" ? "Ok" : AlertType == "warning" ? "Advertencia" : "Error") : title;
        }

        public async Task<IActionResult> OnPostCambiarEstadoAsync(int idUsuario, string codigoEstado)
        {
            try
            {
                await _usuarioService.CambiarEstado(idUsuario, codigoEstado);
                return RedirectToPage("./Index", new
                {
                    success = true,
                    message = "Estado actualizado correctamente.",
                    tipo = "success",
                    title = "Ok"
                });
            }
            catch (Exception ex)
            {
                return RedirectToPage("./Index", new
                {
                    success = true,
                    message = $"Error: {ex.Message}",
                    tipo = "error",
                    title = "Error"
                });
            }
        }

        public async Task<IActionResult> OnPostEliminarAsync(int idUsuario)
        {
            try
            {
                await _usuarioService.Eliminar(idUsuario);
                return RedirectToPage("./Index", new
                {
                    success = true,
                    message = "Usuario eliminado correctamente.",
                    tipo = "success",
                    title = "Ok"
                });
            }
            catch (InvalidOperationException ex)
            {
                return RedirectToPage("./Index", new
                {
                    success = true,
                    message = ex.Message,
                    tipo = "warning",
                    title = "Advertencia"
                });
            }
            catch (Exception ex)
            {
                return RedirectToPage("./Index", new
                {
                    success = true,
                    message = $"Error: {ex.Message}",
                    tipo = "error",
                    title = "Error"
                });
            }
        }
    }
}