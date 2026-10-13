using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PublicaSA.Services.Interfaces;
using PublicaSA.Entities;

namespace PublicaSA.Website.Pages.Reportes.Tareas
{
    public class PorSolicitudModel : PageModel
    {
        private readonly IReporteTareaService _reporteTareaService;

        public PorSolicitudModel(IReporteTareaService reporteTareaService)
        {
            _reporteTareaService = reporteTareaService;
        }

        [BindProperty(SupportsGet = true)]
        [Required(ErrorMessage = "Debe seleccionar una solicitud.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una solicitud.")]
        [Display(Name = "Solicitud")]
        public int? IdSolicitud { get; set; }

        [BindProperty(SupportsGet = true)]
        [Required(ErrorMessage = "Debe seleccionar un mes.")]
        [Range(1, 12, ErrorMessage = "Debe seleccionar un mes válido.")]
        [Display(Name = "Mes")]
        public int? Mes { get; set; }

        [BindProperty(SupportsGet = true)]
        [Required(ErrorMessage = "Debe indicar un año.")]
        [Range(1900, 9999, ErrorMessage = "Debe indicar un año válido.")]
        [Display(Name = "Año")]
        public int? Anio { get; set; }

        public SelectList SolicitudesSelectList { get; set; } = null!;
        public SelectList MesesSelectList { get; set; } = null!;

        public IEnumerable<ReporteTarea> Tareas { get; set; } = new List<ReporteTarea>();
        public decimal TotalHoras { get; set; }
        public bool ReporteGenerado { get; set; }

        public async Task OnGetAsync()
        {
            await CargarCatalogosAsync();

            // Si aún no se han enviado parámetros, solo se muestra el formulario
            if (!IdSolicitud.HasValue && !Mes.HasValue && !Anio.HasValue)
            {
                Anio = DateTime.Today.Year;
                ModelState.Clear();
                return;
            }

            if (!ModelState.IsValid)
            {
                return;
            }

            Tareas = await _reporteTareaService.ObtenerTareasPorSolicitudAsync(IdSolicitud!.Value, Mes!.Value, Anio!.Value);
            TotalHoras = Tareas.Sum(t => t.HorasInvertidas);
            ReporteGenerado = true;
        }

        private async Task CargarCatalogosAsync()
        {
            var solicitudes = await _reporteTareaService.ObtenerSolicitudesAsync();
            var opciones = solicitudes.Select(s => new { s.IdSolicitud, Texto = $"{s.ConsecutivoOficio} - {s.Titulo}" });
            SolicitudesSelectList = new SelectList(opciones, "IdSolicitud", "Texto", IdSolicitud);

            var cultura = new CultureInfo("es-CR");
            var meses = Enumerable.Range(1, 12)
                .Select(m => new { Id = m, Nombre = cultura.TextInfo.ToTitleCase(cultura.DateTimeFormat.GetMonthName(m)) });
            MesesSelectList = new SelectList(meses, "Id", "Nombre", Mes);
        }
    }
}
