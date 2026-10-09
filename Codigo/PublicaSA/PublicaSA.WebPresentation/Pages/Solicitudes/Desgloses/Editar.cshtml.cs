using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PublicaSA.BusinessLogic.Interfaces;
using PublicaSA.Entities;

namespace PublicaSA.WebPresentation.Pages.Solicitudes.Desgloses
{
    public class EditarModel : PageModel
    {
        private readonly IDesgloseService _desgloseService;
        private readonly ISolicitudService _solicitudService;

        public EditarModel(IDesgloseService desgloseService, ISolicitudService solicitudService)
        {
            _desgloseService = desgloseService;
            _solicitudService = solicitudService;
        }

        public Solicitud? Solicitud { get; set; }

        [BindProperty]
        public EditInputModel Input { get; set; } = new EditInputModel();

        public class EditInputModel
        {
            public int IdDesglose { get; set; }

            public int IdSolicitud { get; set; }

            [Required(ErrorMessage = "El mes es requerido.")]
            [Range(1, 12, ErrorMessage = "El mes debe estar entre 1 y 12.")]
            [Display(Name = "Mes")]
            public int Mes { get; set; }

            [Required(ErrorMessage = "El año es requerido.")]
            [Range(2000, 2100, ErrorMessage = "Ingrese un año válido.")]
            [Display(Name = "Año")]
            public int Anio { get; set; }

            [Required(ErrorMessage = "Las horas son requeridas.")]
            [Range(0, 100000, ErrorMessage = "Las horas no pueden ser negativas.")]
            [Display(Name = "Horas")]
            public decimal Horas { get; set; }

            [Required(ErrorMessage = "El monto es requerido.")]
            [Range(0, 10000000000, ErrorMessage = "El monto no puede ser negativo.")]
            [Display(Name = "Monto (₡)")]
            public decimal Monto { get; set; }

            [Required(ErrorMessage = "El porcentaje de cobro es requerido.")]
            [Range(0, 100, ErrorMessage = "El porcentaje debe estar entre 0 y 100.")]
            [Display(Name = "Porcentaje de Cobro (%)")]
            public decimal PorcentajeCobro { get; set; }

            [Display(Name = "Observaciones")]
            [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres.")]
            public string? Observaciones { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id <= 0)
            {
                return RedirectToPage("/Solicitudes/Index");
            }

            var desglose = await _desgloseService.ObtenerPorIdAsync(id);
            if (desglose == null)
            {
                TempData["MensajeError"] = "El desglose especificado no existe.";
                return RedirectToPage("/Solicitudes/Index");
            }

            Solicitud = await _solicitudService.ObtenerPorIdAsync(desglose.IdSolicitud);

            Input = new EditInputModel
            {
                IdDesglose = desglose.IdDesglose,
                IdSolicitud = desglose.IdSolicitud,
                Mes = desglose.Mes,
                Anio = desglose.Anio,
                Horas = desglose.Horas,
                Monto = desglose.Monto,
                PorcentajeCobro = desglose.PorcentajeCobro,
                Observaciones = desglose.Observaciones
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Solicitud = await _solicitudService.ObtenerPorIdAsync(Input.IdSolicitud);
                return Page();
            }

            var desglose = new DesgloseSolicitud
            {
                IdDesglose = Input.IdDesglose,
                IdSolicitud = Input.IdSolicitud,
                Mes = Input.Mes,
                Anio = Input.Anio,
                Horas = Input.Horas,
                Monto = Input.Monto,
                PorcentajeCobro = Input.PorcentajeCobro,
                Observaciones = Input.Observaciones
            };

            string usuario = User.Identity?.Name ?? "admin";
            var resultado = await _desgloseService.ActualizarAsync(desglose, usuario);

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(string.Empty, resultado.Mensaje);
                Solicitud = await _solicitudService.ObtenerPorIdAsync(Input.IdSolicitud);
                return Page();
            }

            TempData["MensajeExito"] = resultado.Mensaje;
            return RedirectToPage("/Solicitudes/Desgloses/Index", new { idSolicitud = Input.IdSolicitud });
        }
    }
}
