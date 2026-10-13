using System;

namespace PublicaSA.Entities
{
    public class ReporteTarea
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal HorasInvertidas { get; set; }
        public string ConsecutivoOficio { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
    }
}
