using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PublicaSA.Entities;
using PublicaSA.Services.Interfaces;

namespace PublicaSA.Website.Pages.Estados
{
    // HU SOL4: listado de estados de solicitudes.
    public class IndexModel : PageModelAutenticado
    {
        private readonly IEstadoSolicitudService _estadoService;

        public IndexModel(IEstadoSolicitudService estadoService)
        {
            _estadoService = estadoService;
        }

        public IEnumerable<EstadoSolicitud> Estados { get; set; } = new List<EstadoSolicitud>();
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; } = 1;
        public int TotalRegistros { get; set; } = 0;
        public const int RegistrosPorPagina = 10;

        [TempData]
        public string? MensajeExito { get; set; }

        [TempData]
        public string? MensajeError { get; set; }

        public bool EsEstadoPorDefecto(string codigo) => _estadoService.EsEstadoPorDefecto(codigo);

        public async Task OnGetAsync(int pagina = 1)
        {
            PaginaActual = pagina < 1 ? 1 : pagina;

            var (items, total) = await _estadoService.ObtenerPaginadoAsync(PaginaActual, RegistrosPorPagina, ObtenerUsuario());
            Estados = items;
            TotalRegistros = total;
            TotalPaginas = (int)Math.Ceiling(total / (double)RegistrosPorPagina);
            if (TotalPaginas < 1) TotalPaginas = 1;
        }

        public async Task<IActionResult> OnPostEliminarAsync(string codigo, int pagina = 1)
        {
            var resultado = await _estadoService.EliminarAsync(codigo, ObtenerUsuario());

            if (resultado.Exitoso)
                MensajeExito = resultado.Mensaje;
            else
                MensajeError = resultado.Mensaje;

            return RedirectToPage(new { pagina });
        }

        private string ObtenerUsuario()
        {
            return HttpContext.Session.GetString("NombreUsuario") ?? User.Identity?.Name ?? "admin";
        }
    }
}
