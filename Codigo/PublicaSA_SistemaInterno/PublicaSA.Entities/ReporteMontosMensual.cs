namespace PublicaSA.Entities
{
    public class ReporteMontosMensual
    {
        // Contiene las solicitudes del mes y año seleccionados.
        public List<MontoSolicitudMensual> Solicitudes { get; set; } = new();

        // Guarda las tres sumas que se muestran al final del reporte.
        public decimal SumaMonto { get; set; }
        public decimal SumaIva { get; set; }
        public decimal SumaTotal { get; set; }
    }
}