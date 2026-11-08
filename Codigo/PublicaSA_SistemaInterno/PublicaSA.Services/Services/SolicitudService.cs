using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PublicaSA.Entities;
using PublicaSA.Repository.Interfaces;
using PublicaSA.Services.Interfaces;
using PublicaSA.Services.Models;

namespace PublicaSA.Services.Services
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

        public async Task<(IEnumerable<Solicitud> Items, int TotalCount)> ObtenerPaginadoAsync(int pagina, int registrosPorPagina, string usuario = "admin")
        {
            if (pagina < 1) pagina = 1;
            if (registrosPorPagina <= 0) registrosPorPagina = 5;

            var resultado = await _solicitudRepository.GetPagedAsync(pagina, registrosPorPagina);

            // Registrar en bitácora según requerimiento: "El usuario consulta <elemento>"
            await _bitacoraService.RegistrarAccionAsync(usuario, "Consultar Solicitudes", "El usuario consulta solicitudes");

            return resultado;
        }

        public async Task<Solicitud?> ObtenerPorIdAsync(int id, string usuario = "admin")
        {
            var resultado = await _solicitudRepository.GetByIdAsync(id);

            if (resultado != null)
            {
                await _bitacoraService.RegistrarAccionAsync(usuario, "Consultar Solicitud", $"El usuario consulta solicitud {resultado.ConsecutivoOficio}");
            }

            return resultado;
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

                // Estado por defecto: "NUEVA"
                if (string.IsNullOrWhiteSpace(solicitud.CodigoEstado))
                {
                    solicitud.CodigoEstado = "NUEVA";
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

                if (string.IsNullOrWhiteSpace(solicitud.CodigoEstado))
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

        public async Task<ResultadoOperacion> ActualizarPublicacionAsync(
            int idSolicitud,
            bool visibleExterno,
            DateTime? fechaVencimientoPublicacion,
            string usuario)
        {
            try
            {
                if (idSolicitud <= 0)
                    return ResultadoOperacion.Error("Identificador de solicitud no válido.");

                var existente = await _solicitudRepository.GetByIdAsync(idSolicitud);
                if (existente == null)
                    return ResultadoOperacion.Error("La solicitud especificada no existe en el sistema.");

                DateTime hoy = DateTimeOffset.UtcNow
                    .ToOffset(TimeSpan.FromHours(-6)).Date;

                if (visibleExterno)
                {
                    if (!fechaVencimientoPublicacion.HasValue)
                    {
                        return ResultadoOperacion.Error("Debe indicar una fecha de vencimiento para publicar la solicitud en el módulo externo.");
                    }

                    if (fechaVencimientoPublicacion.Value.Date < hoy)
                    {
                        return ResultadoOperacion.Error("La fecha de vencimiento no puede ser anterior a la fecha actual.");
                    }
                }
                else
                {
                    // Si se desmarca, se limpia la fecha para consistencia
                    fechaVencimientoPublicacion = null;
                }

                var actualizada = await _solicitudRepository.ActualizarPublicacionAsync(
                    idSolicitud,
                    visibleExterno,
                    fechaVencimientoPublicacion);

                if (actualizada == null)
                {
                    return ResultadoOperacion.Error("No se pudo actualizar el estado de publicación de la solicitud.");
                }

                // Registrar en bitácora según estándar
                await _bitacoraService.RegistrarAccionAsync(
                    usuario,
                    "Publicar Solicitud",
                    new
                    {
                        Accion = visibleExterno ? "Publicación de solicitud a proveedores" : "Retiro de publicación a proveedores",
                        IdSolicitud = idSolicitud,
                        ConsecutivoOficio = existente.ConsecutivoOficio,
                        RegistroAnterior = new
                        {
                            existente.VisibleExterno,
                            existente.FechaVencimientoPublicacion
                        },
                        RegistroActual = new
                        {
                            actualizada.VisibleExterno,
                            actualizada.FechaVencimientoPublicacion
                        }
                    });

                string mensaje = visibleExterno
                    ? $"La solicitud '{existente.ConsecutivoOficio}' se publicó correctamente hasta el {fechaVencimientoPublicacion:dd/MM/yyyy}."
                    : $"La solicitud '{existente.ConsecutivoOficio}' se retiró del módulo externo.";

                return ResultadoOperacion.Exito(mensaje, actualizada);
            }
            catch (Exception ex)
            {
                await _bitacoraService.RegistrarErrorAsync(usuario, "Publicar Solicitud", ex.Message);
                return ResultadoOperacion.Error($"Error al actualizar la publicación de la solicitud: {ex.Message}");
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
