using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PublicaSA.Entities;
using PublicaSA.Services.Interfaces;

namespace PublicaSA.Website.Pages.Solicitudes.Tareas
{
    public class NuevoModel : PageModelAutenticado
    {
        private readonly ITareaService _tareas;

        public NuevoModel(ITareaService tareas)
        {
            _tareas = tareas;
        }

        [BindProperty(SupportsGet = true)]
        public int IdSolicitud { get; set; }

        [BindProperty]
        public FormularioTarea Entrada { get; set; } = new();

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

            if (IdSolicitud <= 0)
            {
                return BadRequest("Debe seleccionar una solicitud.");
            }

            // Selecciona por defecto al usuario que está dentro del sistema.
            Entrada.IdUsuario = idUsuario.Value;
            Entrada.Fecha = DateTime.Today;

            try
            {
                await CargarUsuariosAsync(usuario);
            }
            catch
            {
                ModelState.AddModelError(
                    string.Empty,
                    "No se pudieron cargar los usuarios.");
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

            if (IdSolicitud <= 0)
            {
                return BadRequest("Debe seleccionar una solicitud.");
            }

            try
            {
                // Recarga la lista para validar la selección y mostrar errores.
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
                    IdSolicitud = IdSolicitud,
                    IdUsuario = Entrada.IdUsuario!.Value,
                    Descripcion = Entrada.Descripcion.Trim(),
                    Fecha = Entrada.Fecha!.Value.Date,
                    HorasInvertidas = Entrada.HorasInvertidas!.Value
                };

                await _tareas.CrearAsync(tarea, usuario);

                // Regresa al listado con el mensaje de la operación exitosa.
                TempData["MensajeExitoTareas"] =
                    "La tarea se creó correctamente.";

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
                    "No se pudo guardar la tarea.");
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

        // Valida los campos del formulario antes de crear la tarea.
        public class FormularioTarea : IValidatableObject
        {
            [Required(ErrorMessage = "Ingrese la descripción de la tarea.")]
            [MinLength(3, ErrorMessage =
                "La descripción debe tener al menos tres caracteres.")]
            public string Descripcion { get; set; } = string.Empty;

            [Required(ErrorMessage = "Seleccione la fecha.")]
            [DataType(DataType.Date)]
            public DateTime? Fecha { get; set; }

            [Required(ErrorMessage = "Seleccione un usuario.")]
            [Range(1, int.MaxValue, ErrorMessage =
                "Seleccione un usuario válido.")]
            public int? IdUsuario { get; set; }

            [Required(ErrorMessage = "Ingrese las horas invertidas.")]
            public decimal? HorasInvertidas { get; set; }

            public IEnumerable<ValidationResult> Validate(
                ValidationContext validationContext)
            {
                if (!string.IsNullOrWhiteSpace(Descripcion)
                    && Descripcion.Trim().Length < 3)
                {
                    yield return new ValidationResult(
                        "La descripción debe tener al menos tres caracteres.",
                        new[] { nameof(Descripcion) });
                }

                if (HorasInvertidas.HasValue
                    && (HorasInvertidas.Value <= 0
                        || HorasInvertidas.Value > 24))
                {
                    yield return new ValidationResult(
                        "Las horas invertidas deben ser mayores que cero y hasta 24.",
                        new[] { nameof(HorasInvertidas) });
                }

                if (Fecha.HasValue
                    && (Fecha.Value.Date < new DateTime(2000, 1, 1)
                        || Fecha.Value.Date > new DateTime(2150, 12, 31)))
                {
                    yield return new ValidationResult(
                        "La fecha debe estar entre el 01/01/2000 y el 31/12/2150.",
                        new[] { nameof(Fecha) });
                }
            }
        }
    }
}