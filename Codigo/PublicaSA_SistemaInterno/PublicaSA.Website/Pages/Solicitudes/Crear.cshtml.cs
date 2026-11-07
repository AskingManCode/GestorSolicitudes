using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PublicaSA.Entities;
using PublicaSA.Services.Interfaces;

namespace PublicaSA.Website.Pages.Solicitudes
{
    public class CrearModel : PageModelAutenticado
    {
        private readonly ISolicitudService _solicitudService;

        public CrearModel(ISolicitudService solicitudService)
        {
            _solicitudService = solicitudService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public SelectList RepresentantesSelectList { get; set; } = null!;

        [TempData]
        public string? MensajeError { get; set; }

        public class InputModel
        {
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

            public string CodigoEstado { get; set; } = "NUEVA";

            [Required(ErrorMessage = "La fecha de ingreso es requerida.")]
            [DataType(DataType.Date)]
            [Display(Name = "Fecha de Ingreso")]
            public DateTime FechaIngreso { get; set; } = DateTime.Today;

            [DataType(DataType.Date)]
            [Display(Name = "Fecha de Respuesta")]
            public DateTime? FechaRespuesta { get; set; }

            [DataType(DataType.Date)]
            [Display(Name = "Fecha de Inicio")]
            public DateTime? FechaInicio { get; set; }
        }

        public async Task OnGetAsync()
        {
            await CargarCatalogosAsync();
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
                ConsecutivoOficio = Input.ConsecutivoOficio,
                DocumentoRespuesta = Input.DocumentoRespuesta,
                DocumentoInicio = Input.DocumentoInicio,
                Titulo = Input.Titulo,
                Descripcion = Input.Descripcion,
                IdRepresentante = Input.IdRepresentante,
                Observaciones = Input.Observaciones,
                CodigoEstado = "NUEVA",
                FechaIngreso = Input.FechaIngreso,
                FechaRespuesta = Input.FechaRespuesta,
                FechaInicio = Input.FechaInicio
            };

            string usuario = User.Identity?.Name ?? "admin";
            var resultado = await _solicitudService.CrearAsync(solicitud, usuario);

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
            RepresentantesSelectList = new SelectList(representantes, "IdRepresentante", "Nombre");
        }
    }
}
