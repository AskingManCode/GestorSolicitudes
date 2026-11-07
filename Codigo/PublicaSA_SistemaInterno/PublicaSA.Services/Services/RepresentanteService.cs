using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PublicaSA.Services.Interfaces;
using PublicaSA.Services.Models;
using PublicaSA.Repository.Interfaces;
using PublicaSA.Entities;

namespace PublicaSA.Services.Services
{
    public class RepresentanteService : IRepresentanteService
    {
        private readonly IRepresentanteRepository _representanteRepository;
        private readonly IBitacoraService _bitacoraService;

        public RepresentanteService(
            IRepresentanteRepository representanteRepository,
            IBitacoraService bitacoraService)
        {
            _representanteRepository = representanteRepository;
            _bitacoraService = bitacoraService;
        }

        public async Task<(IEnumerable<Representante> Items, int TotalCount)> ObtenerPaginadoAsync(int pagina, int registrosPorPagina, string usuario)
        {
            if (pagina < 1) pagina = 1;
            if (registrosPorPagina <= 0) registrosPorPagina = 10;

            var resultado = await _representanteRepository.GetPagedAsync(pagina, registrosPorPagina);

            // Registrar en bitácora la consulta
            await _bitacoraService.RegistrarAccionAsync(usuario, "Consultar Representantes", "El usuario consulta representantes");

            return resultado;
        }

        public async Task<Representante?> ObtenerPorIdAsync(int id)
        {
            return await _representanteRepository.GetByIdAsync(id);
        }

        public async Task<ResultadoOperacion> CrearAsync(Representante representante, string usuario)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(representante.Nombre))
                    return ResultadoOperacion.Error("El nombre del representante es requerido.");

                if (string.IsNullOrWhiteSpace(representante.Email))
                    return ResultadoOperacion.Error("El email del representante es requerido.");

                representante.Nombre = representante.Nombre.Trim();
                representante.Email = representante.Email.Trim();
                representante.FechaCreacion = DateTime.Now;

                int nuevoId = await _representanteRepository.InsertAsync(representante);
                representante.IdRepresentante = nuevoId;

                // Registrar en bitácora
                await _bitacoraService.RegistrarAccionAsync(
                    usuario,
                    "Crear Representante",
                    new
                    {
                        Accion = "Creación de representante",
                        NuevoRegistro = representante
                    });

                return ResultadoOperacion.Exito("Representante creado exitosamente.", representante);
            }
            catch (Exception ex)
            {
                await _bitacoraService.RegistrarErrorAsync(usuario, "Crear Representante", ex.Message);
                return ResultadoOperacion.Error($"Error al crear el representante: {ex.Message}");
            }
        }

        public async Task<ResultadoOperacion> ActualizarAsync(Representante representante, string usuario)
        {
            try
            {
                if (representante.IdRepresentante <= 0)
                    return ResultadoOperacion.Error("Identificador de representante no válido.");

                if (string.IsNullOrWhiteSpace(representante.Nombre))
                    return ResultadoOperacion.Error("El nombre del representante es requerido.");

                if (string.IsNullOrWhiteSpace(representante.Email))
                    return ResultadoOperacion.Error("El email del representante es requerido.");

                var existente = await _representanteRepository.GetByIdAsync(representante.IdRepresentante);
                if (existente == null)
                    return ResultadoOperacion.Error("El representante no existe en el sistema.");

                representante.Nombre = representante.Nombre.Trim();
                representante.Email = representante.Email.Trim();
                representante.FechaCreacion = existente.FechaCreacion;

                bool actualizado = await _representanteRepository.UpdateAsync(representante);
                if (!actualizado)
                    return ResultadoOperacion.Error("No se pudo actualizar el representante.");

                // Registrar en bitácora con información anterior y actual
                await _bitacoraService.RegistrarAccionAsync(
                    usuario,
                    "Actualizar Representante",
                    new
                    {
                        Accion = "Actualización de representante",
                        RegistroAnterior = existente,
                        RegistroActual = representante
                    });

                return ResultadoOperacion.Exito("Representante actualizado exitosamente.", representante);
            }
            catch (Exception ex)
            {
                await _bitacoraService.RegistrarErrorAsync(usuario, "Actualizar Representante", ex.Message);
                return ResultadoOperacion.Error($"Error al actualizar el representante: {ex.Message}");
            }
        }

        public async Task<ResultadoOperacion> EliminarAsync(int id, string usuario)
        {
            try
            {
                var existente = await _representanteRepository.GetByIdAsync(id);
                if (existente == null)
                    return ResultadoOperacion.Error("El representante que intenta eliminar no existe.");

                // Validar si está asignado a alguna solicitud
                bool tieneRelacionados = await _representanteRepository.HasRelatedDataAsync(id);
                if (tieneRelacionados)
                {
                    return ResultadoOperacion.Error("No se puede eliminar un registro con datos relacionados.");
                }

                bool eliminado = await _representanteRepository.DeleteAsync(id);
                if (!eliminado)
                    return ResultadoOperacion.Error("No se pudo eliminar el registro.");

                // Registrar en bitácora
                await _bitacoraService.RegistrarAccionAsync(
                    usuario,
                    "Eliminar Representante",
                    new
                    {
                        Accion = "Eliminación de representante",
                        RegistroEliminado = existente
                    });

                return ResultadoOperacion.Exito("Representante eliminado exitosamente.");
            }
            catch (Exception ex)
            {
                await _bitacoraService.RegistrarErrorAsync(usuario, "Eliminar Representante", ex.Message);
                return ResultadoOperacion.Error($"Error al eliminar el representante: {ex.Message}");
            }
        }
    }
}
