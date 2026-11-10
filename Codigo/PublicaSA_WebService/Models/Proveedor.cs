using System;

namespace Models
{
    public class Proveedor
    {
        // Identificador generado por la base de datos.
        public int IdProveedor { get; set; }

        public string CedulaJuridica { get; set; }
        public string Nombre { get; set; }

        // Datos de contacto solicitados al registrar un proveedor nuevo.
        public string NombreRepresentante { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }

        // Los proveedores nuevos se registran como activos.
        public bool Activo { get; set; } = true;

        // La fecha se obtiene del registro guardado en la base de datos.
        public DateTime FechaCreacion { get; set; }
    }
}