using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PublicaSA.BusinessLogic.Interfaces;
using PublicaSA.BusinessLogic.Models;
using PublicaSA.DataAccess.Interfaces;
using PublicaSA.Entities;

namespace PublicaSA.BusinessLogic.Services
{
    public class SolicitudService : ISolicitudService
    {
        private readonly ISolicitudRepository _solicitudRepository;
        private readonly IRepresentanteRepository _representanteRepository;
        private readonly IEstadoSolicitudRepository _estadoRepository;
        private readonly IBitacoraService _bitacoraService;

        public SolicitudService(
            ISolicitudRepository solicitudRepository,
            IRepresentanteRepository representanteRepository,
            IEstadoSolicitudRepository estadoRepository,
            IBitacoraService bitacoraService)
        {
            _solicitudRepository = solicitudRepository;
            _representanteRepository = representanteRepository;
            _estadoRepository = estadoRepository;
            _bitacoraService = bitacoraService;
        }

        public async Task<(IEnumerable<Solicitud> Items, int TotalCount)> ObtenerPaginadoAsync(int pagina, int registrosPorPagina)
        {
            if (pagina < 1) pagina = 1;
            if (registrosPorPagina <= 0) registrosPorPagina = 5;

            return await _solicitudRepository.GetPagedAsync(pagina, registrosPorPagina);
        }

        public async Task<Solicitud?> ObtenerPorIdAsync(int id)
        {
            return await _solicitudRepository.GetByIdAsync(id);
        }

        public async Task<ResultadoOperacion> CrearAsync(Solicitud solicitud, string usuario)
        {
            try
            {
                // Validaciones de negocio
                if (string.IsNullOrWhiteSpace(solicitud.ConsecutivoOficio))
                    return ResultadoOperacion.Error("El consecutivo de oficio es requerido.");

                if (string.IsNullOrWhiteSpace(solicitud.Titulo))
                    return ResultadoOperacion.Error("El título de la solicitud es requerido.");

                if (solicitud.FechaIngreso == default)
                    solicitud.FechaIngreso = DateTime.Today;

                if (solicitud.IdRepresentante <= 0)
                    return ResultadoOperacion.Error("Debe seleccionar un representante válido.");

                // Verificar si el consecutivo ya existe
                var existente = await _solicitudRepository.GetByConsecutivoAsync(solicitud.ConsecutivoOficio.Trim());
                if (existente != null)
                    return ResultadoOperacion.Error($"El consecutivo de oficio '{solicitud.ConsecutivoOficio}' ya se encuentra registrado.");

                // Estado por defecto: "Nueva" (Id = 1)
                if (solicitud.IdEstado <= 0)
                {
                    var estadoNueva = await _estadoRepository.GetByNombreAsync("Nueva");
                    solicitud.IdEstado = estadoNueva?.IdEstado ?? 1;
                }

                solicitud.ConsecutivoOficio = solicitud.ConsecutivoOficio.Trim();
                solicitud.Titulo = solicitud.Titulo.Trim();
                solicitud.FechaCreacion = DateTime.Now;

                int nuevoId = await _solicitudRepository.InsertAsync(solicitud);
                solicitud.IdSolicitud = nuevoId;

                // Registrar en bitácora
                await _bitacoraService.RegistrarAccionAsync(
                    usuario,
                    "Crear Solicitud",
                    new
                    {
                        Accion = "Creación de solicitud",
                        NuevoRegistro = solicitud
                    });

                return ResultadoOperacion.Exito("Solicitud creada exitosamente.", solicitud);
            }
            catch (Exception ex)
            {
                await _bitacoraService.RegistrarErrorAsync(usuario, "Crear Solicitud", ex.Message);
                return ResultadoOperacion.Error($"Error al crear la solicitud: {ex.Message}");
            }
        }

        public async Task<ResultadoOperacion> ActualizarAsync(Solicitud solicitud, string usuario)
        {
            try
            {
                if (solicitud.IdSolicitud <= 0)
                    return ResultadoOperacion.Error("Identificador de solicitud no válido.");

                if (string.IsNullOrWhiteSpace(solicitud.ConsecutivoOficio))
                    return ResultadoOperacion.Error("El consecutivo de oficio es requerido.");

                if (string.IsNullOrWhiteSpace(solicitud.Titulo))
                    return ResultadoOperacion.Error("El título de la solicitud es requerido.");

                if (solicitud.IdRepresentante <= 0)
                    return ResultadoOperacion.Error("Debe seleccionar un representante válido.");

                if (solicitud.IdEstado <= 0)
                    return ResultadoOperacion.Error("Debe seleccionar un estado válido.");

                var existente = await _solicitudRepository.GetByIdAsync(solicitud.IdSolicitud);
                if (existente == null)
                    return ResultadoOperacion.Error("La solicitud no existe en el sistema.");

                // Validar que el consecutivo no esté duplicado con otro registro
                var conMismoConsecutivo = await _solicitudRepository.GetByConsecutivoAsync(solicitud.ConsecutivoOficio.Trim());
                if (conMismoConsecutivo != null && conMismoConsecutivo.IdSolicitud != solicitud.IdSolicitud)
                    return ResultadoOperacion.Error($"El consecutivo de oficio '{solicitud.ConsecutivoOficio}' ya está asignado a otra solicitud.");

                solicitud.ConsecutivoOficio = solicitud.ConsecutivoOficio.Trim();
                solicitud.Titulo = solicitud.Titulo.Trim();

                bool actualizado = await _solicitudRepository.UpdateAsync(solicitud);
                if (!actualizado)
                    return ResultadoOperacion.Error("No se pudo actualizar la solicitud.");

                // Registrar en bitácora con información anterior y actual
                await _bitacoraService.RegistrarAccionAsync(
                    usuario,
                    "Actualizar Solicitud",
                    new
                    {
                        Accion = "Actualización de solicitud",
                        RegistroAnterior = existente,
                        RegistroActual = solicitud
                    });

                return ResultadoOperacion.Exito("Solicitud actualizada exitosamente.", solicitud);
            }
            catch (Exception ex)
            {
                await _bitacoraService.RegistrarErrorAsync(usuario, "Actualizar Solicitud", ex.Message);
                return ResultadoOperacion.Error($"Error al actualizar la solicitud: {ex.Message}");
            }
        }

        public async Task<ResultadoOperacion> EliminarAsync(int id, string usuario)
        {
            try
            {
                var existente = await _solicitudRepository.GetByIdAsync(id);
                if (existente == null)
                    return ResultadoOperacion.Error("La solicitud que intenta eliminar no existe.");

                // Validar si tiene datos relacionados (Desgloses o Tareas)
                bool tieneRelacionados = await _solicitudRepository.HasRelatedDataAsync(id);
                if (tieneRelacionados)
                {
                    return ResultadoOperacion.Error("No se puede eliminar un registro con datos relacionados.");
                }

                bool eliminado = await _solicitudRepository.DeleteAsync(id);
                if (!eliminado)
                    return ResultadoOperacion.Error("No se pudo eliminar el registro.");

                // Registrar en bitácora
                await _bitacoraService.RegistrarAccionAsync(
                    usuario,
                    "Eliminar Solicitud",
                    new
                    {
                        Accion = "Eliminación de solicitud",
                        RegistroEliminado = existente
                    });

                return ResultadoOperacion.Exito("Solicitud eliminada exitosamente.");
            }
            catch (Exception ex)
            {
                await _bitacoraService.RegistrarErrorAsync(usuario, "Eliminar Solicitud", ex.Message);
                return ResultadoOperacion.Error($"Error al eliminar la solicitud: {ex.Message}");
            }
        }

        public async Task<IEnumerable<Representante>> ObtenerRepresentantesAsync()
        {
            return await _representanteRepository.GetAllAsync();
        }

        public async Task<IEnumerable<EstadoSolicitud>> ObtenerEstadosAsync()
        {
            return await _estadoRepository.GetAllAsync();
        }
    }
}
