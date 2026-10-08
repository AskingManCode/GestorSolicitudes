using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PublicaSA.BusinessLogic.Interfaces;
using PublicaSA.Entities;

namespace PublicaSA.WebPresentation.Pages.Solicitudes
{
    public class IndexModel : PageModel
    {
        private readonly ISolicitudService _solicitudService;

        public IndexModel(ISolicitudService solicitudService)
        {
            _solicitudService = solicitudService;
        }

        public IEnumerable<Solicitud> Solicitudes { get; set; } = new List<Solicitud>();
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; } = 1;
        public int TotalRegistros { get; set; } = 0;
        public const int RegistrosPorPagina = 5;

        [TempData]
        public string? MensajeExito { get; set; }

        [TempData]
        public string? MensajeError { get; set; }

        public async Task OnGetAsync(int pagina = 1)
        {
            PaginaActual = pagina < 1 ? 1 : pagina;

            var (items, total) = await _solicitudService.ObtenerPaginadoAsync(PaginaActual, RegistrosPorPagina);
            Solicitudes = items;
            TotalRegistros = total;
            TotalPaginas = (int)Math.Ceiling(total / (double)RegistrosPorPagina);
            if (TotalPaginas < 1) TotalPaginas = 1;
        }

        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            string usuario = User.Identity?.Name ?? "admin";
            var resultado = await _solicitudService.EliminarAsync(id, usuario);

            if (resultado.Exitoso)
            {
                MensajeExito = resultado.Mensaje;
            }
            else
            {
                MensajeError = resultado.Mensaje;
            }

            return RedirectToPage(new { pagina = PaginaActual });
        }
    }
}
