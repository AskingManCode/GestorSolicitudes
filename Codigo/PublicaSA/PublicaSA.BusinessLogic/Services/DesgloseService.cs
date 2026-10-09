using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PublicaSA.BusinessLogic.Interfaces;
using PublicaSA.BusinessLogic.Models;
using PublicaSA.DataAccess.Interfaces;
using PublicaSA.Entities;

namespace PublicaSA.BusinessLogic.Services
{
    public class DesgloseService : IDesgloseService
    {
        private readonly IDesgloseRepository _desgloseRepository;
        private readonly ISolicitudRepository _solicitudRepository;
        private readonly IBitacoraService _bitacoraService;

        public DesgloseService(
            IDesgloseRepository desgloseRepository,
            ISolicitudRepository solicitudRepository,
            IBitacoraService bitacoraService)
        {
            _desgloseRepository = desgloseRepository;
            _solicitudRepository = solicitudRepository;
            _bitacoraService = bitacoraService;
        }

        public async Task<(IEnumerable<DesgloseSolicitud> Items, int TotalCount)> ObtenerPaginadoPorSolicitudAsync(int idSolicitud, int pagina, int registrosPorPagina)
        {
            if (pagina < 1) pagina = 1;
            if (registrosPorPagina <= 0) registrosPorPagina = 10;

            return await _desgloseRepository.GetPagedBySolicitudAsync(idSolicitud, pagina, registrosPorPagina);
        }

        public async Task<IEnumerable<DesgloseSolicitud>> ObtenerPorSolicitudIdAsync(int idSolicitud)
        {
            return await _desgloseRepository.GetBySolicitudIdAsync(idSolicitud);
        }

        public async Task<DesgloseSolicitud?> ObtenerPorIdAsync(int id)
        {
            return await _desgloseRepository.GetByIdAsync(id);
        }

        public async Task<ResultadoOperacion> CrearAsync(DesgloseSolicitud desglose, string usuario)
        {
            try
            {
                // Validar solicitud padre
                var solicitud = await _solicitudRepository.GetByIdAsync(desglose.IdSolicitud);
                if (solicitud == null)
                    return ResultadoOperacion.Error("La solicitud asociada no existe.");

                // Validaciones de negocio
                if (desglose.Mes < 1 || desglose.Mes > 12)
                    return ResultadoOperacion.Error("El mes debe ser un valor entre 1 y 12.");

                if (desglose.Anio < 2000 || desglose.Anio > 2100)
                    return ResultadoOperacion.Error("Debe indicar un año válido.");

                if (desglose.Horas < 0)
                    return ResultadoOperacion.Error("Las horas no pueden ser negativas.");

                if (desglose.Monto < 0)
                    return ResultadoOperacion.Error("El monto no puede ser negativo.");

                if (desglose.PorcentajeCobro < 0 || desglose.PorcentajeCobro > 100)
                    return ResultadoOperacion.Error("El porcentaje de cobro debe estar entre 0% y 100%.");

                // Cálculo automático de IVA (13%) y Total (Monto + IVA)
                desglose.Iva = Math.Round(desglose.Monto * 0.13m, 2);
                desglose.Total = Math.Round(desglose.Monto + desglose.Iva, 2);
                desglose.FechaCreacion = DateTime.Now;

                int nuevoId = await _desgloseRepository.InsertAsync(desglose);
                desglose.IdDesglose = nuevoId;

                // Registro en bitácora
                await _bitacoraService.RegistrarAccionAsync(
                    usuario,
                    "Crear Desglose",
                    new
                    {
                        Accion = "Creación de desglose",
                        NuevoRegistro = desglose
                    });

                return ResultadoOperacion.Exito("Desglose creado exitosamente.", desglose);
            }
            catch (Exception ex)
            {
                await _bitacoraService.RegistrarErrorAsync(usuario, "Crear Desglose", ex.Message);
                return ResultadoOperacion.Error($"Error al crear el desglose: {ex.Message}");
            }
        }

        public async Task<ResultadoOperacion> ActualizarAsync(DesgloseSolicitud desglose, string usuario)
        {
            try
            {
                if (desglose.IdDesglose <= 0)
                    return ResultadoOperacion.Error("Identificador de desglose no válido.");

                var existente = await _desgloseRepository.GetByIdAsync(desglose.IdDesglose);
                if (existente == null)
                    return ResultadoOperacion.Error("El desglose no existe en el sistema.");

                if (desglose.Mes < 1 || desglose.Mes > 12)
                    return ResultadoOperacion.Error("El mes debe ser un valor entre 1 y 12.");

                if (desglose.Anio < 2000 || desglose.Anio > 2100)
                    return ResultadoOperacion.Error("Debe indicar un año válido.");

                if (desglose.Horas < 0)
                    return ResultadoOperacion.Error("Las horas no pueden ser negativas.");

                if (desglose.Monto < 0)
                    return ResultadoOperacion.Error("El monto no puede ser negativo.");

                if (desglose.PorcentajeCobro < 0 || desglose.PorcentajeCobro > 100)
                    return ResultadoOperacion.Error("El porcentaje de cobro debe estar entre 0% y 100%.");

                // Recálculo automático de IVA (13%) y Total (Monto + IVA)
                desglose.Iva = Math.Round(desglose.Monto * 0.13m, 2);
                desglose.Total = Math.Round(desglose.Monto + desglose.Iva, 2);
                desglose.IdSolicitud = existente.IdSolicitud; // Mantener relación con la solicitud padre

                bool actualizado = await _desgloseRepository.UpdateAsync(desglose);
                if (!actualizado)
                    return ResultadoOperacion.Error("No se pudo actualizar el desglose.");

                // Registro en bitácora con información anterior y actual
                await _bitacoraService.RegistrarAccionAsync(
                    usuario,
                    "Actualizar Desglose",
                    new
                    {
                        Accion = "Actualización de desglose",
                        RegistroAnterior = existente,
                        RegistroActual = desglose
                    });

                return ResultadoOperacion.Exito("Desglose actualizado exitosamente.", desglose);
            }
            catch (Exception ex)
            {
                await _bitacoraService.RegistrarErrorAsync(usuario, "Actualizar Desglose", ex.Message);
                return ResultadoOperacion.Error($"Error al actualizar el desglose: {ex.Message}");
            }
        }

        public async Task<ResultadoOperacion> EliminarAsync(int id, string usuario)
        {
            try
            {
                var existente = await _desgloseRepository.GetByIdAsync(id);
                if (existente == null)
                    return ResultadoOperacion.Error("El desglose que intenta eliminar no existe.");

                // Validar si tiene datos relacionados
                bool tieneRelacionados = await _desgloseRepository.HasRelatedDataAsync(id);
                if (tieneRelacionados)
                {
                    return ResultadoOperacion.Error("No se puede eliminar un registro con datos relacionados.");
                }

                bool eliminado = await _desgloseRepository.DeleteAsync(id);
                if (!eliminado)
                    return ResultadoOperacion.Error("No se pudo eliminar el registro.");

                // Registro en bitácora
                await _bitacoraService.RegistrarAccionAsync(
                    usuario,
                    "Eliminar Desglose",
                    new
                    {
                        Accion = "Eliminación de desglose",
                        RegistroEliminado = existente
                    });

                return ResultadoOperacion.Exito("Desglose eliminado exitosamente.");
            }
            catch (Exception ex)
            {
                await _bitacoraService.RegistrarErrorAsync(usuario, "Eliminar Desglose", ex.Message);
                return ResultadoOperacion.Error($"Error al eliminar el desglose: {ex.Message}");
            }
        }
    }
}
