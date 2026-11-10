using System;

namespace Models
{
    public class DesglosePropuesta
    {
        public int IdDesglosePropuesta { get; set; }

        // Relaciona esta línea con la propuesta que la contiene.
        public int IdPropuesta { get; set; }

        public int Mes { get; set; }
        public int Anio { get; set; }

        // Se usa decimal para conservar la precisión de los valores.
        public decimal Horas { get; set; }
        public decimal Monto { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
        public decimal PorcentajeCobro { get; set; }

        public string Observaciones { get; set; }

        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; }
    }
}