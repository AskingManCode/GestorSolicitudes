using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PublicaSA.Entities;
using PublicaSA.Services.Interfaces;

namespace PublicaSA.Website.Pages.Solicitudes.Desgloses
{
    public class CrearModel : PageModelAutenticado
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
            [Display(Name = "Mes")]
            public int Mes { get; set; } = DateTime.Today.Month;

            [Required(ErrorMessage = "El año es requerido.")]
            [Display(Name = "Año")]
            public int Anio { get; set; } = DateTime.Today.Year;

            [Required(ErrorMessage = "Las horas son requeridas.")]
            [Display(Name = "Horas")]
            public decimal Horas { get; set; }

            [Required(ErrorMessage = "El monto es requerido.")]
            [Display(Name = "Monto (₡)")]
            public decimal Monto { get; set; }

            [Display(Name = "IVA (13%)")]
            public decimal Iva { get; set; }

            [Display(Name = "Total")]
            public decimal Total { get; set; }

            [Required(ErrorMessage = "El porcentaje de cobro es requerido.")]
            [Display(Name = "Porcentaje de Cobro (%)")]
            public decimal PorcentajeCobro { get; set; } = 100;

            [Display(Name = "Observaciones")]
            public string? Observaciones { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int idSolicitud)
        {
            if (idSolicitud <= 0)
            {
                return RedirectToPage("/Solicitudes/Index");
            }

            string usuario = User.Identity?.Name ?? "admin";
            Solicitud = await _solicitudService.ObtenerPorIdAsync(idSolicitud, usuario);
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
