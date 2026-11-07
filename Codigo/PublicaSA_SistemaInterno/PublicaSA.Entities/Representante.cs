using System;
using System.Collections.Generic;
using System.Text;

namespace PublicaSA.Entities
{
    public class Representante // Adaptar o borrar clase
    {
        public int IdRepresentante { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
