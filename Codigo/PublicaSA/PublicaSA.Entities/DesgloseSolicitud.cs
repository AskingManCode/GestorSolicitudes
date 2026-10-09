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

        // Propiedad calculada para visualización amigable
        public string NombreMes => Mes switch
        {
            1 => "Enero",
            2 => "Febrero",
            3 => "Marzo",
            4 => "Abril",
            5 => "Mayo",
            6 => "Junio",
            7 => "Julio",
            8 => "Agosto",
            9 => "Septiembre",
            10 => "Octubre",
            11 => "Noviembre",
            12 => "Diciembre",
            _ => Mes.ToString()
        };
    }
}
