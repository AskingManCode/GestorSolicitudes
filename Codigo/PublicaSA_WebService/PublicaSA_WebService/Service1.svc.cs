using BusinessLogic.Services;
using DataAccess.Infrastructure;
using Models;
using System.Configuration;
using System.Diagnostics;
using System;

namespace PublicaSA_WebService
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código, en svc y en el archivo de configuración.
    // NOTE: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Service1.svc o Service1.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class Service1 : IService1
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
        public ResultadoRegistroPropuesta RegistrarPropuesta(
    RegistroPropuestaRequest registro)
        {
            // Lee la conexión definida en Conexiones.local.config.
            var configuracion =
                ConfigurationManager.ConnectionStrings["DefaultConnection"];

            if (configuracion == null ||
                string.IsNullOrWhiteSpace(configuracion.ConnectionString))
            {
                Trace.TraceError(
                    "No está configurada la conexión DefaultConnection.");

                return new ResultadoRegistroPropuesta
                {
                    Exitoso = false,
                    Mensaje = "El servicio no está disponible en este momento."
                };
            }

            // Delega las validaciones y el registro al servicio de negocio.
            var conexionBD = new ConexionBD(configuracion.ConnectionString);
            var servicio = new RegistroPropuestaService(conexionBD);

            return servicio.Registrar(registro);
        }
    }
}
