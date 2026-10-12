using System;
using System.Collections.Generic;
using System.Text;

namespace PublicaSA.Entities
{
    internal class Bitacora // Adaptar o borrar clase
    {
       
        public int IdBitacora { get; set; }
        public DateTime FechaBitacora { get; set; } = DateTime.Now;
        public string UsuarioEjecuta { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
        public string Detalle { get; set; } = string.Empty;
        
    }
}
