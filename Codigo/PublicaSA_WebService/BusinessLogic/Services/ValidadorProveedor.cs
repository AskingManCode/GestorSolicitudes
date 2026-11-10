using System.Text.RegularExpressions;
using Models;

namespace BusinessLogic.Services
{
    public class ValidadorProveedor
    {
        public string ValidarCedula(string cedula)
        {
            if (string.IsNullOrWhiteSpace(cedula))
                return "Debe indicar la cédula jurídica.";

            // Acepta los formatos definidos en la base de datos.
            if (!Regex.IsMatch(cedula, @"\A(3-[0-9]{3}-[0-9]{6}|[0-9]{10})\z"))
                return "La cédula debe tener 10 dígitos o el formato 3-XXX-XXXXXX.";

            return null;
        }

        public string ValidarNuevo(Proveedor proveedor)
        {
            if (proveedor == null)
                return "Debe indicar los datos del proveedor.";

            string errorCedula = ValidarCedula(proveedor.CedulaJuridica);

            if (errorCedula != null)
                return errorCedula;

            if (string.IsNullOrWhiteSpace(proveedor.Nombre) ||
                proveedor.Nombre.Length < 2 ||
                proveedor.Nombre.Length > 150 ||
                proveedor.Nombre != proveedor.Nombre.Trim())
                return "El nombre debe tener entre 2 y 150 caracteres, sin espacios al inicio o al final.";

            if (string.IsNullOrWhiteSpace(proveedor.NombreRepresentante) ||
                proveedor.NombreRepresentante.Length < 3 ||
                proveedor.NombreRepresentante.Length > 150 ||
                proveedor.NombreRepresentante != proveedor.NombreRepresentante.Trim() ||
                Regex.IsMatch(proveedor.NombreRepresentante, "[0-9]"))
                return "El representante debe tener entre 3 y 150 caracteres, sin números ni espacios al inicio o al final.";

            // Permite los mismos caracteres que la tabla proveedores.
            if (string.IsNullOrWhiteSpace(proveedor.Telefono) ||
                proveedor.Telefono.Trim().Length < 8 ||
                !Regex.IsMatch(proveedor.Telefono, @"\A[0-9+() -]{8,30}\z"))
                return "Debe indicar un teléfono válido de entre 8 y 30 caracteres.";

            if (string.IsNullOrWhiteSpace(proveedor.Correo) ||
                proveedor.Correo.Length > 150 ||
                !Regex.IsMatch(
                    proveedor.Correo,
                    @"\A[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\z"))
                return "Debe indicar un correo electrónico válido.";

            return null;
        }
    }
}