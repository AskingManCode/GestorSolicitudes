using System.Collections.Generic;
using Models;

namespace BusinessLogic.Services
{
    public class ValidadorRegistroPropuesta
    {
        public string Validar(RegistroPropuestaRequest registro)
        {
            if (registro == null)
                return "Debe indicar los datos de la propuesta.";

            if (registro.IdSolicitud <= 0)
                return "Debe seleccionar una solicitud válida.";

            if (registro.Proveedor == null)
                return "Debe indicar los datos del proveedor.";

            var validadorProveedor = new ValidadorProveedor();
            string error = validadorProveedor.ValidarCedula(
                registro.Proveedor.CedulaJuridica);

            if (error != null)
                return error;

            if (registro.Desgloses == null ||
                registro.Desgloses.Count == 0)
                return "Debe agregar al menos un desglose.";

            var validadorDesglose = new ValidadorDesglosePropuesta();

            // Guarda los períodos encontrados para detectar repetidos.
            var periodos = new HashSet<string>();

            for (int i = 0; i < registro.Desgloses.Count; i++)
            {
                var desglose = registro.Desgloses[i];
                error = validadorDesglose.Validar(desglose);

                if (error != null)
                    return $"Fila {i + 1}: {error}";

                string periodo = $"{desglose.Anio}-{desglose.Mes}";

                if (!periodos.Add(periodo))
                    return $"Fila {i + 1}: el mes y año ya están incluidos en la propuesta.";
            }

            return null;
        }
    }
}