using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Web.Script.Serialization;
using DataAccess.Infrastructure;
using DataAccess.Repository;
using Models;

namespace BusinessLogic.Services
{
    public class RegistroPropuestaService
    {
        private readonly ConexionBD _conexionBD;

        public RegistroPropuestaService(ConexionBD conexionBD)
        {
            _conexionBD = conexionBD;
        }

        public ResultadoRegistroPropuesta Registrar(
            RegistroPropuestaRequest registro)
        {
            var validador = new ValidadorRegistroPropuesta();
            string error = validador.Validar(registro);

            if (error != null)
                return Fallo(error);

            try
            {
                using (var conexion = _conexionBD.CrearConexion())
                {
                    conexion.Open();

                    using (var transaccion = conexion.BeginTransaction(
                        IsolationLevel.ReadCommitted))
                    {
                        var solicitudes = new SolicitudRepository();
                        var proveedores = new ProveedorRepository(_conexionBD);
                        var propuestas = new PropuestaRepository();
                        var desgloses = new DesglosePropuestaRepository();
                        var bitacora = new BitacoraRepository();
                        var serializador = new JavaScriptSerializer();

                        // Revisa la publicación con la fecha de Costa Rica.
                        DateTime hoy = DateTimeOffset.UtcNow
                            .ToOffset(TimeSpan.FromHours(-6)).Date;

                        if (!solicitudes.EstaDisponibleParaParticipar(
                            registro.IdSolicitud,
                            hoy,
                            conexion,
                            transaccion))
                        {
                            return Fallo(
                                "La solicitud no está disponible para participar.");
                        }

                        // Las cédulas nuevas se guardan sin guiones.
                        string cedula = registro.Proveedor.CedulaJuridica
                            .Replace("-", "");

                        var proveedor = proveedores.ObtenerPorCedula(
                            cedula,
                            conexion,
                            transaccion);

                        if (proveedor == null)
                        {
                            var validadorProveedor = new ValidadorProveedor();

                            error = validadorProveedor.ValidarNuevo(
                                registro.Proveedor);

                            if (error != null)
                                return Fallo(error);

                            // Usa solo los datos necesarios del formulario.
                            proveedor = new Proveedor
                            {
                                CedulaJuridica = cedula,
                                Nombre = registro.Proveedor.Nombre,
                                NombreRepresentante =
                                    registro.Proveedor.NombreRepresentante,
                                Telefono = registro.Proveedor.Telefono,
                                Correo = registro.Proveedor.Correo,
                                Activo = true
                            };

                            proveedor.IdProveedor = proveedores.Crear(
                                proveedor,
                                conexion,
                                transaccion);

                            bitacora.Registrar(
                                "Proveedor externo",
                                null,
                                "CREAR",
                                "El proveedor se registra. " +
                                serializador.Serialize(new
                                {
                                    proveedor.IdProveedor,
                                    proveedor.CedulaJuridica,
                                    proveedor.Nombre,
                                    proveedor.NombreRepresentante,
                                    proveedor.Telefono,
                                    proveedor.Correo,
                                    proveedor.Activo
                                }),
                                conexion,
                                transaccion);
                        }

                        if (!proveedor.Activo)
                            return Fallo("El proveedor está inactivo.");

                        if (propuestas.ExisteParaSolicitud(
                            registro.IdSolicitud,
                            proveedor.IdProveedor,
                            conexion,
                            transaccion))
                        {
                            return Fallo(
                                "El proveedor ya tiene una propuesta para esta solicitud.");
                        }

                        int idPropuesta = propuestas.Crear(
                            registro.IdSolicitud,
                            proveedor.IdProveedor,
                            conexion,
                            transaccion);

                        foreach (var desglose in registro.Desgloses)
                        {
                            // Los IDs se obtienen durante el guardado.
                            desglose.IdPropuesta = idPropuesta;

                            desglose.IdDesglosePropuesta = desgloses.Crear(
                                idPropuesta,
                                desglose,
                                conexion,
                                transaccion);
                        }

                        bitacora.Registrar(
                            "Proveedor externo",
                            null,
                            "CREAR",
                            "El proveedor registra una propuesta. " +
                            serializador.Serialize(new
                            {
                                IdPropuesta = idPropuesta,
                                registro.IdSolicitud,
                                proveedor.IdProveedor,
                                proveedor.CedulaJuridica,
                                Aprobada = false,

                                // Excluye la fecha del cliente y refleja
                                // el estado activo usado en el INSERT.
                                Desgloses = registro.Desgloses.Select(d => new
                                {
                                    d.IdDesglosePropuesta,
                                    d.IdPropuesta,
                                    d.Mes,
                                    d.Anio,
                                    d.Horas,
                                    d.Monto,
                                    d.Iva,
                                    d.Total,
                                    d.PorcentajeCobro,
                                    d.Observaciones,
                                    Activo = true
                                }).ToList()
                            }),
                            conexion,
                            transaccion);

                        // Confirma la propuesta, los desgloses y la bitácora.
                        transaccion.Commit();

                        return new ResultadoRegistroPropuesta
                        {
                            Exitoso = true,
                            Mensaje = "La propuesta se registró correctamente.",
                            IdPropuesta = idPropuesta
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                // Deja el detalle técnico disponible para diagnóstico.
                Trace.TraceError(
                    "Error al registrar la propuesta: {0}", ex);

                return Fallo(
                    "No se pudo registrar la propuesta. Intentá nuevamente.");
            }
        }

        private ResultadoRegistroPropuesta Fallo(string mensaje)
        {
            return new ResultadoRegistroPropuesta
            {
                Exitoso = false,
                Mensaje = mensaje
            };
        }
    }
}