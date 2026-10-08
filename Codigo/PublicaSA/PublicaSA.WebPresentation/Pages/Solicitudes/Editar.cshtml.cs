using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PublicaSA.BusinessLogic.Interfaces;
using PublicaSA.Entities;

namespace PublicaSA.WebPresentation.Pages.Solicitudes
{
    public class EditarModel : PageModel
    {
        private readonly ISolicitudService _solicitudService;

        public EditarModel(ISolicitudService solicitudService)
        {
            _solicitudService = solicitudService;
        }

        [BindProperty]
        public EditInputModel Input { get; set; } = new EditInputModel();

        public SelectList RepresentantesSelectList { get; set; } = null!;
        public SelectList EstadosSelectList { get; set; } = null!;

        public class EditInputModel
        {
            public int IdSolicitud { get; set; }

            [Required(ErrorMessage = "El consecutivo de oficio es requerido.")]
            [StringLength(50, ErrorMessage = "El consecutivo no puede exceder 50 caracteres.")]
            [Display(Name = "Consecutivo de Oficio")]
            public string ConsecutivoOficio { get; set; } = string.Empty;

            [StringLength(255)]
            [Display(Name = "Documento de Respuesta")]
            public string? DocumentoRespuesta { get; set; }

            [StringLength(255)]
            [Display(Name = "Documento de Inicio")]
            public string? DocumentoInicio { get; set; }

            [Required(ErrorMessage = "El título de la solicitud es requerido.")]
            [StringLength(200, ErrorMessage = "El título no puede exceder 200 caracteres.")]
            [Display(Name = "Título")]
            public string Titulo { get; set; } = string.Empty;

            [Display(Name = "Descripción")]
            public string? Descripcion { get; set; }

            [Required(ErrorMessage = "Debe seleccionar un representante.")]
            [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un representante válido.")]
            [Display(Name = "Representante")]
            public int IdRepresentante { get; set; }

            [Display(Name = "Observaciones")]
            public string? Observaciones { get; set; }

            [Required(ErrorMessage = "Debe seleccionar un estado.")]
            [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un estado válido.")]
            [Display(Name = "Estado")]
            public int IdEstado { get; set; }

            [Required(ErrorMessage = "La fecha de ingreso es requerida.")]
            [DataType(DataType.Date)]
            [Display(Name = "Fecha de Ingreso")]
            public DateTime FechaIngreso { get; set; }

            [DataType(DataType.Date)]
            [Display(Name = "Fecha de Respuesta")]
            public DateTime? FechaRespuesta { get; set; }

            [DataType(DataType.Date)]
            [Display(Name = "Fecha de Inicio")]
            public DateTime? FechaInicio { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id <= 0)
            {
                return RedirectToPage("/Solicitudes/Index");
            }

            var solicitud = await _solicitudService.ObtenerPorIdAsync(id);
            if (solicitud == null)
            {
                TempData["MensajeError"] = "La solicitud especificada no existe.";
                return RedirectToPage("/Solicitudes/Index");
            }

            Input = new EditInputModel
            {
                IdSolicitud = solicitud.IdSolicitud,
                ConsecutivoOficio = solicitud.ConsecutivoOficio,
                DocumentoRespuesta = solicitud.DocumentoRespuesta,
                DocumentoInicio = solicitud.DocumentoInicio,
                Titulo = solicitud.Titulo,
                Descripcion = solicitud.Descripcion,
                IdRepresentante = solicitud.IdRepresentante,
                Observaciones = solicitud.Observaciones,
                IdEstado = solicitud.IdEstado,
                FechaIngreso = solicitud.FechaIngreso,
                FechaRespuesta = solicitud.FechaRespuesta,
                FechaInicio = solicitud.FechaInicio
            };

            await CargarCatalogosAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await CargarCatalogosAsync();
                return Page();
            }

            var solicitud = new Solicitud
            {
                IdSolicitud = Input.IdSolicitud,
                ConsecutivoOficio = Input.ConsecutivoOficio,
                DocumentoRespuesta = Input.DocumentoRespuesta,
                DocumentoInicio = Input.DocumentoInicio,
                Titulo = Input.Titulo,
                Descripcion = Input.Descripcion,
                IdRepresentante = Input.IdRepresentante,
                Observaciones = Input.Observaciones,
                IdEstado = Input.IdEstado,
                FechaIngreso = Input.FechaIngreso,
                FechaRespuesta = Input.FechaRespuesta,
                FechaInicio = Input.FechaInicio
            };

            string usuario = User.Identity?.Name ?? "admin";
            var resultado = await _solicitudService.ActualizarAsync(solicitud, usuario);

            if (!resultado.Exitoso)
            {
                ModelState.AddModelError(string.Empty, resultado.Mensaje);
                await CargarCatalogosAsync();
                return Page();
            }

            TempData["MensajeExito"] = resultado.Mensaje;
            return RedirectToPage("/Solicitudes/Index");
        }

        private async Task CargarCatalogosAsync()
        {
            var representantes = await _solicitudService.ObtenerRepresentantesAsync();
            RepresentantesSelectList = new SelectList(representantes, "IdRepresentante", "Nombre", Input.IdRepresentante);

            var estados = await _solicitudService.ObtenerEstadosAsync();
            EstadosSelectList = new SelectList(estados, "IdEstado", "Nombre", Input.IdEstado);
        }
    }
}
