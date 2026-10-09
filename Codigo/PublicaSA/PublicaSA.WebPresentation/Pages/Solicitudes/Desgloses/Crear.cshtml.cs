using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PublicaSA.BusinessLogic.Interfaces;
using PublicaSA.Entities;

namespace PublicaSA.WebPresentation.Pages.Solicitudes.Desgloses
{
    public class CrearModel : PageModel
    {
        private readonly IDesgloseService _desgloseService;
        private readonly ISolicitudService _solicitudService;

        public CrearModel(IDesgloseService desgloseService, ISolicitudService solicitudService)
        {
            _desgloseService = desgloseService;
            _solicitudService = solicitudService;
        }

        public Solicitud? Solicitud { get; set; }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required]
            public int IdSolicitud { get; set; }

            [Required(ErrorMessage = "El mes es requerido.")]
            [Range(1, 12, ErrorMessage = "El mes debe estar entre 1 y 12.")]
            [Display(Name = "Mes")]
            public int Mes { get; set; } = DateTime.Today.Month;

            [Required(ErrorMessage = "El año es requerido.")]
            [Range(2000, 2100, ErrorMessage = "Ingrese un año válido.")]
            [Display(Name = "Año")]
            public int Anio { get; set; } = DateTime.Today.Year;

            [Required(ErrorMessage = "Las horas son requeridas.")]
            [Range(0, 100000, ErrorMessage = "Las horas no pueden ser negativas.")]
            [Display(Name = "Horas")]
            public decimal Horas { get; set; }

            [Required(ErrorMessage = "El monto es requerido.")]
            [Range(0, 10000000000, ErrorMessage = "El monto no puede ser negativo.")]
            [Display(Name = "Monto (₡)")]
            public decimal Monto { get; set; }

            [Display(Name = "IVA (13%)")]
            public decimal Iva { get; set; }

            [Display(Name = "Total")]
            public decimal Total { get; set; }

            [Required(ErrorMessage = "El porcentaje de cobro es requerido.")]
            [Range(0, 100, ErrorMessage = "El porcentaje debe estar entre 0 y 100.")]
            [Display(Name = "Porcentaje de Cobro (%)")]
            public decimal PorcentajeCobro { get; set; } = 100;

            [Display(Name = "Observaciones")]
            [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres.")]
            public string? Observaciones { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int idSolicitud)
        {
            if (idSolicitud <= 0)
            {
                return RedirectToPage("/Solicitudes/Index");
            }

            Solicitud = await _solicitudService.ObtenerPorIdAsync(idSolicitud);
            if (Solicitud == null)
            {
                TempData["MensajeError"] = "La solicitud indicada no existe.";
                return RedirectToPage("/Solicitudes/Index");
            }

            Input.IdSolicitud = idSolicitud;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Solicitud = await _solicitudService.ObtenerPorIdAsync(Input.IdSolicitud);
            if (Solicitud == null)
            {
                TempData["MensajeError"] = "La solicitud asociada no existe.";
                return RedirectToPage("/Solicitudes/Index");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var desglose = new DesgloseSolicitud
            {
                IdSolicitud = Input.IdSolicitud,
                Mes = Input.Mes,
                Anio = Input.Anio,
                Horas = Input.Horas,
                Monto = Input.Monto,
                PorcentajeCobro = Input.PorcentajeCobro,
                Observaciones = Input.Observaciones
            };

            string usuario = User.Identity?.Name ?? "admin";
            var resultado = await _desgloseService.CrearAsync(desglose, usuario);

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(string.Empty, resultado.Mensaje);
                return Page();
            }

            TempData["MensajeExito"] = resultado.Mensaje;
            return RedirectToPage("/Solicitudes/Desgloses/Index", new { idSolicitud = Input.IdSolicitud });
        }
    }
}
