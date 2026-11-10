using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PublicaSA.Entities;
using PublicaSA.Services.Interfaces;

namespace PublicaSA.Website.Pages.Estados
{
    // HU SOL4: formulario para editar un estado de solicitud.
    public class EditarModel : PageModelAutenticado
    {
        private readonly IEstadoSolicitudService _estadoService;

        public EditarModel(IEstadoSolicitudService estadoService)
        {
            _estadoService = estadoService;
        }

        [BindProperty]
        public EditInputModel Input { get; set; } = new EditInputModel();

        public class EditInputModel
        {
            // El identificador es la llave del estado; se muestra pero no se modifica.
            public string CodigoEstado { get; set; } = string.Empty;

            [Required(ErrorMessage = "El nombre es requerido.")]
            [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
            [Display(Name = "Nombre")]
            public string Nombre { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync(string codigo)
        {
            var estado = await _estadoService.ObtenerPorCodigoAsync(codigo);
            if (estado == null)
            {
                TempData["MensajeError"] = "El estado especificado no existe.";
                return RedirectToPage("/Estados/Index");
            }

            Input = new EditInputModel
            {
                CodigoEstado = estado.CodigoEstado,
                Nombre = estado.Nombre
            };

            return Page();
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
            var resultado = await _estadoService.ActualizarAsync(estado, usuario);

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
