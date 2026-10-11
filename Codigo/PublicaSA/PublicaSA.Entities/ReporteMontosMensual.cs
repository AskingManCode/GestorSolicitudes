namespace PublicaSA.Entities
{
    // Agrupa las filas del reporte y sus sumas generales.
    public class ReporteMontosMensual
    {
        public List<MontoSolicitudMensual> Solicitudes { get; set; } = new();

        public decimal TotalMonto { get; set; }

        public decimal TotalIva { get; set; }

        public decimal TotalGeneral { get; set; }
    }
}