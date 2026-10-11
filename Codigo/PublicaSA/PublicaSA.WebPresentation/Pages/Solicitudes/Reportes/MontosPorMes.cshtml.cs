using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PublicaSA.BusinessLogic.Interfaces;
using PublicaSA.Entities;
using System.ComponentModel.DataAnnotations;

namespace PublicaSA.WebPresentation.Pages.Solicitudes.Reportes
{
    public class MontosPorMesModel : PageModel
    {
        private readonly IReporteMontosService _reportes;

        public MontosPorMesModel(IReporteMontosService reportes)
        {
            _reportes = reportes;
        }

        [BindProperty]
        public ParametrosReporte Entrada { get; set; } = new();

        public ReporteMontosMensual? Reporte { get; private set; }

        // Valida los parámetros antes de consultar la base de datos.
        public class ParametrosReporte
        {
            [Required(ErrorMessage = "Seleccione el mes.")]
            [Range(1, 12, ErrorMessage = "Seleccione un mes válido.")]
            public int? Mes { get; set; }

            [Required(ErrorMessage = "Indique el año.")]
            [Range(1, 65535, ErrorMessage = "Indique un año válido.")]
            public int? Anio { get; set; }
        }

        public IActionResult OnGet()
        {
            int? idUsuarioActual =
                HttpContext.Session.GetInt32("IdUsuario");

            if (!idUsuarioActual.HasValue || idUsuarioActual.Value <= 0)
            {
                return RedirigirAlLogin();
            }

            // Al abrir la página muestra el formulario sin generar el reporte.
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            int? idUsuarioActual =
                HttpContext.Session.GetInt32("IdUsuario");

            if (!idUsuarioActual.HasValue || idUsuarioActual.Value <= 0)
            {
                return RedirigirAlLogin();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // El servicio obtiene las filas, suma los importes y registra la consulta.
                Reporte = await _reportes.GenerarAsync(
                    Entrada.Mes!.Value,
                    Entrada.Anio!.Value,
                    idUsuarioActual.Value);
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "No se pudo generar el reporte de montos.");
            }

            return Page();
        }

        private IActionResult RedirigirAlLogin()
        {
            TempData["Mensaje"] =
                "Por favor inicie sesión para utilizar el sistema";

            return RedirectToPage("/Login/Login");
        }
    }
}