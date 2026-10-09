using System;

namespace PublicaSA.Entities
{
    public class Bitacora
    {
        public int IdBitacora { get; set; }
        public DateTime FechaBitacora { get; set; } = DateTime.Now;
        public string UsuarioEjecuta { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
        public string Detalle { get; set; } = string.Empty;
    }
}
