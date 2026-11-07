namespace PublicaSA.Entities
{
    // Representa una fila del reporte mensual solicitado en SOL7.
    public class MontoSolicitudMensual
    {
        public string ConsecutivoOficio { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public int Mes { get; set; }
        public int Anio { get; set; }

        // Decimal conserva la precisión de los importes monetarios.
        public decimal Monto { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
    }
}