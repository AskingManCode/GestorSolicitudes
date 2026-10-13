using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using PublicaSA.Entities;
using PublicaSA.Services.Interfaces;

namespace PublicaSA.Website.Pages.Solicitudes.Tareas
{
    public class IndexModel : PageModel
    {
        private readonly ITareaService _tareas;

        public IndexModel(ITareaService tareas)
        {
            _tareas = tareas;
        }

        [BindProperty(SupportsGet = true)]
        public int IdSolicitud { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Pagina { get; set; } = 1;

        public IEnumerable<Tarea> Tareas { get; private set; }
            = Array.Empty<Tarea>();

        public int TotalPaginas { get; private set; }

        public string? Error { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!TieneSesion())
            {
                return RedirigirAlLogin();
            }

            if (IdSolicitud <= 0)
            {
                return BadRequest("Debe seleccionar una solicitud.");
            }

            if (Pagina < 1)
            {
                Pagina = 1;
            }

            string usuario = HttpContext.Session
                .GetString("NombreUsuario")!;

            try
            {
                var resultado = await _tareas.ListarPorSolicitudAsync(
                    IdSolicitud,
                    Pagina,
                    usuario);

                Tareas = resultado.Tareas;

                // Cada página contiene un máximo de diez tareas.
                TotalPaginas = Math.Max(
                    1,
                    (int)Math.Ceiling(resultado.Total / 10.0));
            }
            catch
            {
                Error = "No se pudieron consultar las tareas.";
            }

            return Page();
        }

        // Se ejecuta al confirmar Sí en el formulario de eliminación.
        public async Task<IActionResult> OnPostEliminarAsync(int idTarea)
        {
            if (!TieneSesion())
            {
                return RedirigirAlLogin();
            }

            if (IdSolicitud <= 0 || idTarea <= 0)
            {
                return BadRequest(
                    "Debe seleccionar una tarea y su solicitud.");
            }

            string usuario = HttpContext.Session
                .GetString("NombreUsuario")!;

            try
            {
                bool eliminada = await _tareas.EliminarAsync(
                    idTarea,
                    IdSolicitud,
                    usuario);

                if (eliminada)
                {
                    TempData["MensajeExitoTareas"] =
                        "La tarea se eliminó correctamente.";
                }
                else
                {
                    TempData["MensajeErrorTareas"] =
                        "No se encontró la tarea seleccionada.";
                }
            }
            catch (MySqlException error) when (error.Number == 1451)
            {
                // Una clave foránea impide borrar un registro relacionado.
                TempData["MensajeErrorTareas"] =
                    "No se puede eliminar un registro con datos relacionados.";
            }
            catch
            {
                TempData["MensajeErrorTareas"] =
                    "No se pudo eliminar la tarea.";
            }
            
            return RedirectToPage(
                "./Index",
                new { idSolicitud = IdSolicitud });
        }

        private bool TieneSesion()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            string? usuario = HttpContext.Session.GetString("NombreUsuario");

            return idUsuario.HasValue
                && idUsuario.Value > 0
                && !string.IsNullOrWhiteSpace(usuario);
        }

        private IActionResult RedirigirAlLogin()
        {
            TempData["Mensaje"] =
                "Por favor inicie sesión para utilizar el sistema";

            return RedirectToPage("/Login/Index");
        }
    }
}