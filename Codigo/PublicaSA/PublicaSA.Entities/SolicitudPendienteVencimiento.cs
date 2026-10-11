namespace PublicaSA.Entities
{
    // Contiene los datos necesarios para revisar el vencimiento.
    public class SolicitudPendienteVencimiento
    {
        public int IdSolicitud { get; set; }

        public DateTime FechaIngreso { get; set; }

        public int IdEstadoSolicitud { get; set; }
    }
}