using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PublicaSA.Entities;
using PublicaSA.Repository.Interfaces;
using PublicaSA.Services.Interfaces;
using PublicaSA.Services.Models;

namespace PublicaSA.Services.Services
{
    // HU SOL4: lógica del mantenimiento de estados de solicitudes.
    public class EstadoSolicitudService : IEstadoSolicitudService
    {
        // Estados por defecto que pide la HU SOL4. El sistema depende de ellos
        // (SOL1 crea las solicitudes en NUEVA y SOL5 las marca como VENCIDA),
        // por eso no se permite eliminarlos.
        private static readonly string[] EstadosPorDefecto =
        {
            "NUEVA", "RESPONDIDA", "INICIADA", "FINALIZADA", "VENCIDA"
        };

        // Mismas reglas que las restricciones CHECK de la tabla EstadosSolicitud.
        private static readonly Regex FormatoCodigo = new Regex(@"^[A-Z_]{2,20}$");

        private readonly IEstadoSolicitudRepository _estadoRepository;
        private readonly IBitacoraService _bitacoraService;

        public EstadoSolicitudService(
            IEstadoSolicitudRepository estadoRepository,
            IBitacoraService bitacoraService)
        {
            _estadoRepository = estadoRepository;
            _bitacoraService = bitacoraService;
        }

        public bool EsEstadoPorDefecto(string codigo)
        {
            return EstadosPorDefecto.Contains((codigo ?? string.Empty).Trim().ToUpperInvariant());
        }

        public async Task<(IEnumerable<EstadoSolicitud> Items, int TotalCount)> ObtenerPaginadoAsync(int pagina, int registrosPorPagina, string usuario)
        {
            if (pagina < 1) pagina = 1;
            if (registrosPorPagina <= 0) registrosPorPagina = 10;

            var resultado = await _estadoRepository.GetPagedAsync(pagina, registrosPorPagina);

            await _bitacoraService.RegistrarAccionAsync(usuario, "Consultar Estados de Solicitud", "El usuario consulta estados de solicitud");

            return resultado;
        }

        public async Task<EstadoSolicitud?> ObtenerPorCodigoAsync(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return null;

            return await _estadoRepository.GetByCodigoAsync(codigo.Trim().ToUpperInvariant());
        }

        public async Task<ResultadoOperacion> CrearAsync(EstadoSolicitud estado, string usuario)
        {
            try
            {
                string? error = ValidarCodigo(estado.CodigoEstado) ?? ValidarNombre(estado.Nombre);
                if (error != null)
                    return ResultadoOperacion.Error(error);

                estado.CodigoEstado = estado.CodigoEstado.Trim().ToUpperInvariant();
                estado.Nombre = estado.Nombre.Trim();

                if (await _estadoRepository.GetByCodigoAsync(estado.CodigoEstado) != null)
                    return ResultadoOperacion.Error("Ya existe un estado con el identificador indicado.");

                if (await _estadoRepository.GetByNombreAsync(estado.Nombre) != null)
                    return ResultadoOperacion.Error("Ya existe un estado con el nombre indicado.");

                estado.Activo = true;
                estado.FechaCreacion = DateTime.Now;

                bool creado = await _estadoRepository.InsertAsync(estado);
                if (!creado)
                    return ResultadoOperacion.Error("No se pudo crear el estado.");

                await _bitacoraService.RegistrarAccionAsync(
                    usuario,
                    "Crear Estado de Solicitud",
                    new
                    {
                        Accion = "Creación de estado de solicitud",
                        NuevoRegistro = estado
                    });

                return ResultadoOperacion.Exito("Estado creado exitosamente.", estado);
            }
            catch (Exception ex)
            {
                await _bitacoraService.RegistrarErrorAsync(usuario, "Crear Estado de Solicitud", ex.Message);
                return ResultadoOperacion.Error($"Error al crear el estado: {ex.Message}");
            }
        }

        public async Task<ResultadoOperacion> ActualizarAsync(EstadoSolicitud estado, string usuario)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(estado.CodigoEstado))
                    return ResultadoOperacion.Error("Identificador de estado no válido.");

                string? error = ValidarNombre(estado.Nombre);
                if (error != null)
                    return ResultadoOperacion.Error(error);

                estado.CodigoEstado = estado.CodigoEstado.Trim().ToUpperInvariant();
                estado.Nombre = estado.Nombre.Trim();

                var existente = await _estadoRepository.GetByCodigoAsync(estado.CodigoEstado);
                if (existente == null)
                    return ResultadoOperacion.Error("El estado no existe en el sistema.");

                var mismoNombre = await _estadoRepository.GetByNombreAsync(estado.Nombre);
                if (mismoNombre != null && mismoNombre.CodigoEstado != estado.CodigoEstado)
                    return ResultadoOperacion.Error("Ya existe un estado con el nombre indicado.");

                estado.Activo = existente.Activo;
                estado.FechaCreacion = existente.FechaCreacion;

                bool actualizado = await _estadoRepository.UpdateAsync(estado);
                if (!actualizado)
                    return ResultadoOperacion.Error("No se pudo actualizar el estado.");

                await _bitacoraService.RegistrarAccionAsync(
                    usuario,
                    "Actualizar Estado de Solicitud",
                    new
                    {
                        Accion = "Actualización de estado de solicitud",
                        RegistroAnterior = existente,
                        RegistroActual = estado
                    });

                return ResultadoOperacion.Exito("Estado actualizado exitosamente.", estado);
            }
            catch (Exception ex)
            {
                await _bitacoraService.RegistrarErrorAsync(usuario, "Actualizar Estado de Solicitud", ex.Message);
                return ResultadoOperacion.Error($"Error al actualizar el estado: {ex.Message}");
            }
        }

        public async Task<ResultadoOperacion> EliminarAsync(string codigo, string usuario)
        {
            try
            {
                var existente = await ObtenerPorCodigoAsync(codigo);
                if (existente == null)
                    return ResultadoOperacion.Error("El estado que intenta eliminar no existe.");

                // Criterio de aceptación: solo se elimina si no ha sido asignado.
                if (await _estadoRepository.HasRelatedDataAsync(existente.CodigoEstado))
                    return ResultadoOperacion.Error("No se puede eliminar un registro con datos relacionados.");

                if (EsEstadoPorDefecto(existente.CodigoEstado))
                    return ResultadoOperacion.Error("No se puede eliminar un estado por defecto del sistema.");

                bool eliminado = await _estadoRepository.DeleteAsync(existente.CodigoEstado);
                if (!eliminado)
                    return ResultadoOperacion.Error("No se pudo eliminar el registro.");

                await _bitacoraService.RegistrarAccionAsync(
                    usuario,
                    "Eliminar Estado de Solicitud",
                    new
                    {
                        Accion = "Eliminación de estado de solicitud",
                        RegistroEliminado = existente
                    });

                return ResultadoOperacion.Exito("Estado eliminado exitosamente.");
            }
            catch (Exception ex)
            {
                await _bitacoraService.RegistrarErrorAsync(usuario, "Eliminar Estado de Solicitud", ex.Message);
                return ResultadoOperacion.Error($"Error al eliminar el estado: {ex.Message}");
            }
        }

        private static string? ValidarCodigo(string? codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return "El identificador del estado es requerido.";

            if (!FormatoCodigo.IsMatch(codigo.Trim().ToUpperInvariant()))
                return "El identificador debe tener entre 2 y 20 letras sin tildes (A-Z) o guion bajo, sin espacios. Ejemplo: EN_REVISION.";

            return null;
        }

        private static string? ValidarNombre(string? nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return "El nombre del estado es requerido.";

            string limpio = nombre.Trim();
            if (limpio.Length < 2 || limpio.Length > 50)
                return "El nombre del estado debe tener entre 2 y 50 caracteres.";

            return null;
        }
    }
}
