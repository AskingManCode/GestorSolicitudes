using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PublicaSA.Services.Interfaces;
using PublicaSA.Entities;

namespace PublicaSA.Website.Pages.Representantes
{
    public class IndexModel : PageModelAutenticado
    {
        private readonly IRepresentanteService _representanteService;

        public IndexModel(IRepresentanteService representanteService)
        {
            _representanteService = representanteService;
        }

        public IEnumerable<Representante> Representantes { get; set; } = new List<Representante>();
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; } = 1;
        public int TotalRegistros { get; set; } = 0;
        public const int RegistrosPorPagina = 10;

        [TempData]
        public string? MensajeExito { get; set; }

        [TempData]
        public string? MensajeError { get; set; }

        public async Task OnGetAsync(int pagina = 1)
        {
            PaginaActual = pagina < 1 ? 1 : pagina;

            string usuario = User.Identity?.Name ?? "admin";
            var (items, total) = await _representanteService.ObtenerPaginadoAsync(PaginaActual, RegistrosPorPagina, usuario);
            Representantes = items;
            TotalRegistros = total;
            TotalPaginas = (int)Math.Ceiling(total / (double)RegistrosPorPagina);
            if (TotalPaginas < 1) TotalPaginas = 1;
        }

        public async Task<IActionResult> OnPostEliminarAsync(int id)
        {
            string usuario = User.Identity?.Name ?? "admin";
            var resultado = await _representanteService.EliminarAsync(id, usuario);

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
