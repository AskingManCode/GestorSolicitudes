using System;
using Models;

namespace BusinessLogic.Services
{
    public class ValidadorDesglosePropuesta
    {
        public string Validar(DesglosePropuesta desglose)
        {
            if (desglose == null)
                return "Debe indicar los datos del desglose.";

            if (desglose.Mes < 1 || desglose.Mes > 12)
                return "El mes debe estar entre 1 y 12.";

            if (desglose.Anio < 2000 || desglose.Anio > 2150)
                return "El año debe estar entre 2000 y 2150.";

            // Los límites corresponden a las columnas de la base de datos.
            if (desglose.Horas < 0 || desglose.Horas > 99999999.99m)
                return "Las horas están fuera del rango permitido.";

            if (desglose.Monto < 0 ||
                desglose.Monto > 999999999999.99m)
                return "El monto está fuera del rango permitido.";

            if (desglose.PorcentajeCobro < 0 ||
                desglose.PorcentajeCobro > 100)
                return "El porcentaje de cobro debe estar entre 0 y 100.";

            if (string.IsNullOrWhiteSpace(desglose.Observaciones))
                return "Debe indicar las observaciones del desglose.";

            // Evita que la base de datos redondee los datos silenciosamente.
            if (TieneMasDeDosDecimales(desglose.Horas) ||
                TieneMasDeDosDecimales(desglose.Monto) ||
                TieneMasDeDosDecimales(desglose.PorcentajeCobro))
                return "Las horas, el monto y el porcentaje admiten hasta dos decimales.";

            decimal ivaEsperado = Math.Round(
                desglose.Monto * 0.13m,
                2,
                MidpointRounding.AwayFromZero);

            decimal totalEsperado = desglose.Monto + ivaEsperado;

            if (totalEsperado > 999999999999.99m)
                return "El total supera el máximo permitido.";

            // También comprueba los valores recibidos desde Excel.
            if (desglose.Iva != ivaEsperado)
                return "El IVA debe corresponder al 13 % del monto, redondeado a dos decimales.";

            if (desglose.Total != totalEsperado)
                return "El total debe ser igual al monto más el IVA.";

            return null;
        }

        private bool TieneMasDeDosDecimales(decimal valor)
        {
            return valor != Math.Round(valor, 2);
        }
    }
}