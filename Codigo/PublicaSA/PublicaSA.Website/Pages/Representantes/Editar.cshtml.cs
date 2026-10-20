using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PublicaSA.Services.Interfaces;
using PublicaSA.Entities;

namespace PublicaSA.Website.Pages.Representantes
{
    public class EditarModel : PageModelAutenticado
    {
        private readonly IRepresentanteService _representanteService;

        public EditarModel(IRepresentanteService representanteService)
        {
            _representanteService = representanteService;
        }

        [BindProperty]
        public EditInputModel Input { get; set; } = new EditInputModel();

        public class EditInputModel
        {
            public int IdRepresentante { get; set; }

            [Required(ErrorMessage = "El nombre es requerido.")]
            [StringLength(150, ErrorMessage = "El nombre no puede exceder 150 caracteres.")]
            [Display(Name = "Nombre")]
            public string Nombre { get; set; } = string.Empty;

            [Required(ErrorMessage = "El email es requerido.")]
            [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
            [StringLength(150, ErrorMessage = "El email no puede exceder 150 caracteres.")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id <= 0)
            {
                return RedirectToPage("/Representantes/Index");
            }

            var representante = await _representanteService.ObtenerPorIdAsync(id);
            if (representante == null)
            {
                TempData["MensajeError"] = "El representante especificado no existe.";
                return RedirectToPage("/Representantes/Index");
            }

            Input = new EditInputModel
            {
                IdRepresentante = representante.IdRepresentante,
                Nombre = representante.Nombre,
                Email = representante.Email
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var representante = new Representante
            {
                IdRepresentante = Input.IdRepresentante,
                Nombre = Input.Nombre,
                Email = Input.Email
            };

            string usuario = User.Identity?.Name ?? "admin";
            var resultado = await _representanteService.ActualizarAsync(representante, usuario);

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(string.Empty, resultado.Mensaje);
                return Page();
            }

            TempData["MensajeExito"] = resultado.Mensaje;
            return RedirectToPage("/Representantes/Index");
        }
    }
}
