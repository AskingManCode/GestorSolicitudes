using System;

namespace PublicaSA.Entities
{
    public class Representante
    {
        public int IdRepresentante { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
