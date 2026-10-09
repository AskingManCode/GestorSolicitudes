using System;

namespace PublicaSA.Entities
{
    public class DesgloseSolicitud
    {
        public int IdDesglose { get; set; }
        public int IdSolicitud { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
        public decimal Horas { get; set; }
        public decimal Monto { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
        public decimal PorcentajeCobro { get; set; }
        public string? Observaciones { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
