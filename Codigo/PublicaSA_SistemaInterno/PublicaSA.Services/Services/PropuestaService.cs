using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PublicaSA.Entities;
using PublicaSA.Repository.Interfaces;
using PublicaSA.Services.Interfaces;
using PublicaSA.Services.Models;

namespace PublicaSA.Services.Services
{
    public class PropuestaService : IPropuestaService
    {
        private readonly IPropuestaRepository _propuestaRepository;
        private readonly IBitacoraService _bitacoraService;

        public PropuestaService(
            IPropuestaRepository propuestaRepository,
            IBitacoraService bitacoraService)
        {
            _propuestaRepository = propuestaRepository;
            _bitacoraService = bitacoraService;
        }

        public async Task<IEnumerable<Propuesta>> ObtenerPorSolicitudAsync(int idSolicitud, string usuario = "admin")
        {
            try
            {
                var propuestas = await _propuestaRepository.ListarPorSolicitudAsync(idSolicitud);

                await _bitacoraService.RegistrarAccionAsync(
                    usuario,
                    "Consultar Participaciones",
                    $"El usuario consulta participaciones de la solicitud {idSolicitud}");

                return propuestas;
            }
            catch (Exception ex)
            {
                await _bitacoraService.RegistrarErrorAsync(usuario, "Consultar Participaciones", ex.Message);
                throw;
            }
        }

        public async Task<(Propuesta? Propuesta, IEnumerable<DesglosePropuesta> Desgloses)> ObtenerDetalleAsync(int idPropuesta, string usuario = "admin")
        {
            try
            {
                var resultado = await _propuestaRepository.ObtenerDetalleAsync(idPropuesta);

                if (resultado.Propuesta != null)
                {
                    await _bitacoraService.RegistrarAccionAsync(
                        usuario,
                        "Consultar Detalle de Participación",
                        $"El usuario consulta el desglose propuesto por el proveedor {resultado.Propuesta.CedulaJuridica} para la solicitud {resultado.Propuesta.ConsecutivoOficio}");
                }

                return resultado;
            }
            catch (Exception ex)
            {
                await _bitacoraService.RegistrarErrorAsync(usuario, "Consultar Detalle de Participación", ex.Message);
                throw;
            }
        }

        public async Task<ResultadoOperacion> AprobarAsync(int idPropuesta, int idSolicitud, string usuario = "admin")
        {
            try
            {
                if (idPropuesta <= 0 || idSolicitud <= 0)
                    return ResultadoOperacion.Error("Identificadores de participación o solicitud no válidos.");

                var (propuestaExistente, desglosesPropuestos) = await _propuestaRepository.ObtenerDetalleAsync(idPropuesta);

                if (propuestaExistente == null || propuestaExistente.IdSolicitud != idSolicitud)
                    return ResultadoOperacion.Error("La participación seleccionada no existe para esta solicitud.");

                if (propuestaExistente.SolicitudTienePropuestaAprobada)
                    return ResultadoOperacion.Error("Ya se aprobó una propuesta para esta solicitud.");

                var listaDesgloses = desglosesPropuestos.ToList();
                if (!listaDesgloses.Any())
                    return ResultadoOperacion.Error("La propuesta seleccionada no tiene desgloses registrados para asociar a la solicitud.");

                var propuestaAprobada = await _propuestaRepository.AprobarAsync(idPropuesta, idSolicitud);
                if (propuestaAprobada == null)
                    return ResultadoOperacion.Error("No se pudo aprobar la participación seleccionada.");

                // Registrar en bitácora según requerimiento de actualización (anterior y actual en JSON)
                await _bitacoraService.RegistrarAccionAsync(
                    usuario,
                    "Aprobar Participación",
                    new
                    {
                        Accion = "Aprobación de participación de proveedor y asociación de desglose a la solicitud",
                        RegistroAnterior = propuestaExistente,
                        RegistroActual = propuestaAprobada,
                        DesglosesAsociados = listaDesgloses
                    });

                return ResultadoOperacion.Exito(
                    $"La participación del proveedor '{propuestaAprobada.NombreProveedor}' ({propuestaAprobada.CedulaJuridica}) fue aprobada exitosamente y su desglose quedó asociado a la solicitud.",
                    propuestaAprobada);
            }
            catch (Exception ex)
            {
                await _bitacoraService.RegistrarErrorAsync(usuario, "Aprobar Participación", ex.Message);
                return ResultadoOperacion.Error($"Error al aprobar la participación: {ex.Message}");
            }
        }
    }
}
