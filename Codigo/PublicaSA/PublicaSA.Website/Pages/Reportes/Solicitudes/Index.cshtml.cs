using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PublicaSA.Entities;
using PublicaSA.Services.Interfaces;

namespace PublicaSA.Website.Pages.Reportes.Solicitudes
{
    public class IndexModel : PageModelAutenticado
    {
        private readonly IReporteMontosService _reportes;

        public IndexModel(IReporteMontosService reportes)
        {
            _reportes = reportes;
        }

        [BindProperty]
        [Required(ErrorMessage = "Seleccione el mes.")]
        [Range(1, 12, ErrorMessage = "El mes debe estar entre 1 y 12.")]
        public int? Mes { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Ingrese el año.")]
        [Range(2000, 2150, ErrorMessage =
            "El año debe estar entre 2000 y 2150.")]
        public int? Anio { get; set; }

        public ReporteMontosMensual? Reporte { get; private set; }

        public IActionResult OnGet()
        {
            if (!TieneSesion())
            {
                return RedirigirAlLogin();
            }

            // Propone el período actual; el usuario puede cambiarlo.
            DateTime hoy = DateTimeOffset.UtcNow
                .ToOffset(TimeSpan.FromHours(-6)).Date;

            Mes = hoy.Month;
            Anio = hoy.Year;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!TieneSesion())
            {
                return RedirigirAlLogin();
            }

            // No consulta la base si faltan parámetros o son inválidos.
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string usuario = HttpContext.Session
                .GetString("NombreUsuario")!;

            try
            {
                Reporte = await _reportes.GenerarAsync(
                    Mes!.Value,
                    Anio!.Value,
                    usuario);
            }
            catch
            {
                ModelState.AddModelError(
                    string.Empty,
                    "No se pudo generar el reporte.");
            }

            return Page();
        }

        private bool TieneSesion()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            string? usuario = HttpContext.Session.GetString("NombreUsuario");

            return idUsuario.HasValue
                && idUsuario.Value > 0
                && !string.IsNullOrWhiteSpace(usuario);
        }

        private IActionResult RedirigirAlLogin()
        {
            TempData["Mensaje"] =
                "Por favor inicie sesión para utilizar el sistema";

            return RedirectToPage("/Login/Index");
        }
    }
}