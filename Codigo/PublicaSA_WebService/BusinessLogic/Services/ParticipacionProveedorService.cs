using System;
using System.Diagnostics;
using DataAccess.Infrastructure;
using DataAccess.Repository;
using Models;

namespace BusinessLogic.Services
{
    // HU EXT6: participaciones de un proveedor en solicitudes.
    public class ParticipacionProveedorService
    {
        public const string MensajeProveedorNoExiste = "Proveedor no existe.";
        public const string MensajeSinDatos = "Datos no encontrados.";

        private readonly ParticipacionProveedorRepository _participaciones;
        private readonly ValidadorProveedor _validador = new ValidadorProveedor();

        public ParticipacionProveedorService(ConexionBD conexionBD)
        {
            _participaciones = new ParticipacionProveedorRepository(conexionBD);
        }

        public ResultadoParticipacionesProveedor Consultar(string cedulaJuridica)
        {
            string cedula = (cedulaJuridica ?? string.Empty).Trim();

            // Mismos formatos de cédula jurídica que en EXT4.
            string errorFormato = _validador.ValidarCedula(cedula);
            if (errorFormato != null)
                return new ResultadoParticipacionesProveedor { Exitoso = false, Mensaje = errorFormato };

            var proveedor = _participaciones.ObtenerProveedorPorCedula(cedula);
            if (proveedor == null)
                return new ResultadoParticipacionesProveedor { Exitoso = false, Mensaje = MensajeProveedorNoExiste };

            var resultado = new ResultadoParticipacionesProveedor
            {
                NombreProveedor = proveedor.Nombre,
                Participaciones = _participaciones.ListarPorProveedor(proveedor.IdProveedor)
            };

            resultado.Exitoso = resultado.Participaciones.Count > 0;
            resultado.Mensaje = resultado.Exitoso
                ? "Consulta realizada correctamente."
                : MensajeSinDatos;

            try
            {
                _participaciones.RegistrarConsulta(proveedor.CedulaJuridica);
            }
            catch (Exception ex)
            {
                // Un fallo de la bitácora no impide mostrar la consulta.
                Trace.TraceError("No se pudo registrar la bitácora de EXT6: " + ex.Message);
            }

            return resultado;
        }
    }
}
