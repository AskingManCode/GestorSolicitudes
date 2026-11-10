using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PublicaSA.Entities;
using PublicaSA.Services.Interfaces;

namespace PublicaSA.Website.Pages.Estados
{
    // HU SOL4: formulario para crear un estado de solicitud.
    public class CrearModel : PageModelAutenticado
    {
        private readonly IEstadoSolicitudService _estadoService;

        public CrearModel(IEstadoSolicitudService estadoService)
        {
            _estadoService = estadoService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required(ErrorMessage = "El identificador es requerido.")]
            [RegularExpression(@"^[A-Za-z_]{2,20}$",
                ErrorMessage = "El identificador debe tener entre 2 y 20 letras sin tildes (A-Z) o guion bajo, sin espacios. Ejemplo: EN_REVISION.")]
            [Display(Name = "Identificador")]
            public string CodigoEstado { get; set; } = string.Empty;

            [Required(ErrorMessage = "El nombre es requerido.")]
            [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
            [Display(Name = "Nombre")]
            public string Nombre { get; set; } = string.Empty;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var estado = new EstadoSolicitud
            {
                CodigoEstado = Input.CodigoEstado,
                Nombre = Input.Nombre
            };

            string usuario = HttpContext.Session.GetString("NombreUsuario") ?? User.Identity?.Name ?? "admin";
            var resultado = await _estadoService.CrearAsync(estado, usuario);

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(string.Empty, resultado.Mensaje);
                return Page();
            }

            TempData["MensajeExito"] = resultado.Mensaje;
            return RedirectToPage("/Estados/Index");
        }
    }
}
