using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PublicaSA.BusinessLogic.Interfaces;
using PublicaSA.Entities;

namespace PublicaSA.WebPresentation.Pages.Solicitudes.Desgloses
{
    public class IndexModel : PageModel
    {
        private readonly IDesgloseService _desgloseService;
        private readonly ISolicitudService _solicitudService;

        public IndexModel(IDesgloseService desgloseService, ISolicitudService solicitudService)
        {
            _desgloseService = desgloseService;
            _solicitudService = solicitudService;
        }

        public Solicitud? Solicitud { get; set; }
        public IEnumerable<DesgloseSolicitud> Desgloses { get; set; } = new List<DesgloseSolicitud>();

        [BindProperty(SupportsGet = true)]
        public int IdSolicitud { get; set; }

        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; } = 1;
        public int TotalRegistros { get; set; } = 0;
        public const int RegistrosPorPagina = 10;

        [TempData]
        public string? MensajeExito { get; set; }

        [TempData]
        public string? MensajeError { get; set; }

        public async Task<IActionResult> OnGetAsync(int idSolicitud, int pagina = 1)
        {
            if (idSolicitud <= 0)
            {
                return RedirectToPage("/Solicitudes/Index");
            }

            IdSolicitud = idSolicitud;
            Solicitud = await _solicitudService.ObtenerPorIdAsync(idSolicitud);
            if (Solicitud == null)
            {
                TempData["MensajeError"] = "La solicitud indicada no existe.";
                return RedirectToPage("/Solicitudes/Index");
            }

            PaginaActual = pagina < 1 ? 1 : pagina;

            var (items, total) = await _desgloseService.ObtenerPaginadoPorSolicitudAsync(idSolicitud, PaginaActual, RegistrosPorPagina);
            Desgloses = items;
            TotalRegistros = total;
            TotalPaginas = (int)Math.Ceiling(total / (double)RegistrosPorPagina);
            if (TotalPaginas < 1) TotalPaginas = 1;

            return Page();
        }

        public async Task<IActionResult> OnPostEliminarAsync(int id, int idSolicitud)
        {
            string usuario = User.Identity?.Name ?? "admin";
            var resultado = await _desgloseService.EliminarAsync(id, usuario);

            if (resultado.Exitoso)
            {
                MensajeExito = resultado.Mensaje;
            }
            else
            {
                MensajeError = resultado.Mensaje;
            }

            return RedirectToPage(new { idSolicitud, pagina = PaginaActual });
        }
    }
}
