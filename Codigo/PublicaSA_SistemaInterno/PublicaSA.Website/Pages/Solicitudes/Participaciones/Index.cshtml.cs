using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PublicaSA.Entities;
using PublicaSA.Services.Interfaces;

namespace PublicaSA.Website.Pages.Solicitudes.Participaciones
{
    public class IndexModel : PageModelAutenticado
    {
        private readonly IPropuestaService _propuestaService;
        private readonly ISolicitudService _solicitudService;

        public IndexModel(IPropuestaService propuestaService, ISolicitudService solicitudService)
        {
            _propuestaService = propuestaService;
            _solicitudService = solicitudService;
        }

        [BindProperty(SupportsGet = true)]
        public int IdSolicitud { get; set; }

        public Solicitud? Solicitud { get; set; }
        public IEnumerable<Propuesta> Participaciones { get; set; } = new List<Propuesta>();

        [TempData]
        public string? MensajeExito { get; set; }

        [TempData]
        public string? MensajeError { get; set; }

        public async Task<IActionResult> OnGetAsync(int idSolicitud)
        {
            if (idSolicitud <= 0)
            {
                return RedirectToPage("/Solicitudes/Index");
            }

            IdSolicitud = idSolicitud;
            string usuario = HttpContext.Session.GetString("NombreUsuario") ?? User.Identity?.Name ?? "admin";

            Solicitud = await _solicitudService.ObtenerPorIdAsync(idSolicitud, usuario);
            if (Solicitud == null)
            {
                TempData["MensajeError"] = "La solicitud indicada no existe.";
                return RedirectToPage("/Solicitudes/Index");
            }

            try
            {
                Participaciones = await _propuestaService.ObtenerPorSolicitudAsync(idSolicitud, usuario);
            }
            catch (Exception ex)
            {
                MensajeError = $"No se pudieron consultar las participaciones: {ex.Message}";
            }

            return Page();
        }
    }
}
