using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PublicaSA.Entities;
using PublicaSA.Services.Interfaces;

namespace PublicaSA.Website.Pages.Solicitudes.Tareas
{
    public class EditarModel : PageModel
    {
        private readonly ITareaService _tareas;

        public EditarModel(ITareaService tareas)
        {
            _tareas = tareas;
        }

        [BindProperty(SupportsGet = true)]
        public int IdSolicitud { get; set; }

        [BindProperty(SupportsGet = true)]
        public int IdTarea { get; set; }

        // Reutiliza las mismas validaciones del formulario Nuevo.
        [BindProperty]
        public NuevoModel.FormularioTarea Entrada { get; set; } = new();

        public List<SelectListItem> Usuarios { get; private set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            string? usuario = HttpContext.Session.GetString("NombreUsuario");

            if (!idUsuario.HasValue || idUsuario.Value <= 0
                || string.IsNullOrWhiteSpace(usuario))
            {
                return RedirigirAlLogin();
            }

            if (IdSolicitud <= 0 || IdTarea <= 0)
            {
                return BadRequest("Debe seleccionar una tarea y su solicitud.");
            }

            try
            {
                var tarea = await _tareas.ObtenerPorIdAsync(
                    IdTarea,
                    IdSolicitud,
                    usuario);

                if (tarea is null)
                {
                    return NotFound();
                }

                // Precarga los valores que podrá modificar el usuario.
                Entrada = new NuevoModel.FormularioTarea
                {
                    Descripcion = tarea.Descripcion,
                    Fecha = tarea.Fecha.Date,
                    IdUsuario = tarea.IdUsuario,
                    HorasInvertidas = tarea.HorasInvertidas
                };

                await CargarUsuariosAsync(usuario);
            }
            catch
            {
                TempData["MensajeErrorTareas"] =
                    "No se pudo cargar la tarea.";

                return RedirectToPage(
                    "./Index",
                    new { idSolicitud = IdSolicitud });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            string? usuario = HttpContext.Session.GetString("NombreUsuario");

            if (!idUsuario.HasValue || idUsuario.Value <= 0
                || string.IsNullOrWhiteSpace(usuario))
            {
                return RedirigirAlLogin();
            }

            if (IdSolicitud <= 0 || IdTarea <= 0)
            {
                return BadRequest("Debe seleccionar una tarea y su solicitud.");
            }

            try
            {
                await CargarUsuariosAsync(usuario);

                if (Entrada.IdUsuario.HasValue
                    && !Usuarios.Any(u =>
                        u.Value == Entrada.IdUsuario.Value.ToString()))
                {
                    ModelState.AddModelError(
                        "Entrada.IdUsuario",
                        "Seleccione un usuario válido.");
                }

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var tarea = new Tarea
                {
                    IdTarea = IdTarea,
                    IdSolicitud = IdSolicitud,
                    IdUsuario = Entrada.IdUsuario!.Value,
                    Descripcion = Entrada.Descripcion.Trim(),
                    Fecha = Entrada.Fecha!.Value.Date,
                    HorasInvertidas = Entrada.HorasInvertidas!.Value
                };

                // El repositorio verifica que la tarea pertenezca a la solicitud.
                bool actualizada = await _tareas.ActualizarAsync(
                    tarea,
                    usuario);

                if (!actualizada)
                {
                    return NotFound();
                }

                TempData["MensajeExitoTareas"] =
                    "La tarea se actualizó correctamente.";

                return RedirectToPage(
                    "./Index",
                    new { idSolicitud = IdSolicitud });
            }
            catch (ArgumentException error)
            {
                ModelState.AddModelError(string.Empty, error.Message);
            }
            catch
            {
                ModelState.AddModelError(
                    string.Empty,
                    "No se pudo actualizar la tarea.");
            }

            return Page();
        }

        private async Task CargarUsuariosAsync(string usuarioActual)
        {
            var usuarios = await _tareas.ListarUsuariosAsync(usuarioActual);

            Usuarios = usuarios.Select(usuario => new SelectListItem
            {
                Value = usuario.IdUsuario.ToString(),
                Text = usuario.NombreCompleto
            }).ToList();
        }

        private IActionResult RedirigirAlLogin()
        {
            TempData["Mensaje"] =
                "Por favor inicie sesión para utilizar el sistema";

            return RedirectToPage("/Login/Index");
        }
    }
}