using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PublicaSA.Services.Interfaces;
using PublicaSA.Entities;

namespace PublicaSA.Website.Pages.Representantes
{
    public class CrearModel : PageModel
    {
        private readonly IRepresentanteService _representanteService;

        public CrearModel(IRepresentanteService representanteService)
        {
            _representanteService = representanteService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
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

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var representante = new Representante
            {
                Nombre = Input.Nombre,
                Email = Input.Email
            };

            string usuario = User.Identity?.Name ?? "admin";
            var resultado = await _representanteService.CrearAsync(representante, usuario);

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
