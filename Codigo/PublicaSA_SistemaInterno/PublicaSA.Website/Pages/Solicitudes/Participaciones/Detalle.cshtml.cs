using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PublicaSA.Entities;
using PublicaSA.Services.Interfaces;

namespace PublicaSA.Website.Pages.Solicitudes.Participaciones
{
    public class DetalleModel : PageModelAutenticado
    {
        private readonly IPropuestaService _propuestaService;

        public DetalleModel(IPropuestaService propuestaService)
        {
            _propuestaService = propuestaService;
        }

        [BindProperty(SupportsGet = true)]
        public int IdPropuesta { get; set; }

        [BindProperty(SupportsGet = true)]
        public int IdSolicitud { get; set; }

        public Propuesta? Propuesta { get; set; }
        public IEnumerable<DesglosePropuesta> Desgloses { get; set; } = new List<DesglosePropuesta>();

        [TempData]
        public string? MensajeExito { get; set; }

        [TempData]
        public string? MensajeError { get; set; }

        public async Task<IActionResult> OnGetAsync(int idPropuesta, int idSolicitud)
        {
            if (idPropuesta <= 0 || idSolicitud <= 0)
            {
                return RedirectToPage("/Solicitudes/Index");
            }

            IdPropuesta = idPropuesta;
            IdSolicitud = idSolicitud;
            string usuario = HttpContext.Session.GetString("NombreUsuario") ?? User.Identity?.Name ?? "admin";

            try
            {
                var (propuesta, desgloses) = await _propuestaService.ObtenerDetalleAsync(idPropuesta, usuario);

                if (propuesta == null || propuesta.IdSolicitud != idSolicitud)
                {
                    TempData["MensajeError"] = "La participación seleccionada no existe para esta solicitud.";
                    return RedirectToPage("./Index", new { idSolicitud });
                }

                Propuesta = propuesta;
                Desgloses = desgloses;
                return Page();
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"No se pudo cargar el detalle de la participación: {ex.Message}";
                return RedirectToPage("./Index", new { idSolicitud });
            }
        }

        public async Task<IActionResult> OnPostAprobarAsync(int idPropuesta, int idSolicitud)
        {
            if (idPropuesta <= 0 || idSolicitud <= 0)
            {
                return RedirectToPage("/Solicitudes/Index");
            }

            string usuario = HttpContext.Session.GetString("NombreUsuario") ?? User.Identity?.Name ?? "admin";
            var resultado = await _propuestaService.AprobarAsync(idPropuesta, idSolicitud, usuario);

            if (resultado.Exitoso)
            {
                MensajeExito = resultado.Mensaje;
            }
            else
            {
                MensajeError = resultado.Mensaje;
            }

            return RedirectToPage("./Detalle", new { idPropuesta, idSolicitud });
        }
    }
}
