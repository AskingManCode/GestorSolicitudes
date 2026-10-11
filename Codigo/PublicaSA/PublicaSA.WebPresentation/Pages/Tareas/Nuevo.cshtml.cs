using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PublicaSA.BusinessLogic.Interfaces;
using PublicaSA.Entities;
using System.ComponentModel.DataAnnotations;

namespace PublicaSA.WebPresentation.Pages.Tareas
{
    public class NuevoModel : PageModel
    {
        private readonly ITareaSolicitudService _tareas;

        public NuevoModel(ITareaSolicitudService tareas)
        {
            _tareas = tareas;
        }

        [BindProperty(SupportsGet = true)]
        public int IdSolicitud { get; set; }

        [BindProperty]
        public DatosTarea Entrada { get; set; } = new();

        public List<SelectListItem> Usuarios { get; private set; } = new();

        // Define los campos y las validaciones del formulario.
        public class DatosTarea
        {
            [Required(ErrorMessage = "Indique la descripción.")]
            [StringLength(500,
                ErrorMessage = "La descripción admite hasta 500 caracteres.")]
            public string Descripcion { get; set; } = string.Empty;

            [Required(ErrorMessage = "Indique la fecha.")]
            [DataType(DataType.Date)]
            public DateTime? Fecha { get; set; }

            [Required(ErrorMessage = "Seleccione un usuario.")]
            public int? IdUsuario { get; set; }

            [Required(ErrorMessage = "Indique las horas invertidas.")]
            public decimal? HorasInvertidas { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            int? idUsuarioActual =
                HttpContext.Session.GetInt32("IdUsuario");

            if (!idUsuarioActual.HasValue || idUsuarioActual.Value <= 0)
            {
                return RedirigirAlLogin();
            }

            if (IdSolicitud <= 0)
            {
                return BadRequest();
            }

            // Selecciona por defecto al usuario que inició sesión.
            Entrada.IdUsuario = idUsuarioActual.Value;

            try
            {
                await CargarUsuariosAsync(idUsuarioActual.Value);
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "No se pudieron cargar los usuarios.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Comprueba la sesión antes de procesar el guardado.
            int? idUsuarioActual =
                HttpContext.Session.GetInt32("IdUsuario");

            if (!idUsuarioActual.HasValue || idUsuarioActual.Value <= 0)
            {
                return RedirigirAlLogin();
            }

            if (IdSolicitud <= 0)
            {
                return BadRequest();
            }

            try
            {
                await CargarUsuariosAsync(idUsuarioActual.Value);

                if (Entrada.IdUsuario.HasValue &&
                    !Usuarios.Any(usuario =>
                        usuario.Value == Entrada.IdUsuario.Value.ToString()))
                {
                    ModelState.AddModelError(
                        "Entrada.IdUsuario",
                        "Seleccione un usuario existente.");
                }

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var tarea = new TareaSolicitud
                {
                    IdSolicitud = IdSolicitud,
                    Descripcion = Entrada.Descripcion,
                    Fecha = Entrada.Fecha!.Value.Date,
                    IdUsuario = Entrada.IdUsuario!.Value,
                    HorasInvertidas = Entrada.HorasInvertidas!.Value
                };

                // El servicio guarda la tarea mediante el repositorio.
                await _tareas.CrearAsync(tarea, idUsuarioActual.Value);

                TempData["Exito"] = "La tarea se registró correctamente.";

                return RedirectToPage(
                    "./Index",
                    new { idSolicitud = IdSolicitud });
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "No se pudo guardar la tarea.");
            }

            return Page();
        }

        private async Task CargarUsuariosAsync(int idUsuarioActual)
        {
            var usuarios = await _tareas.ListarUsuariosAsync(
                idUsuarioActual);

            // Muestra el nombre y utiliza el identificador como valor.
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

            return RedirectToPage("/Login/Login");
        }
    }
}