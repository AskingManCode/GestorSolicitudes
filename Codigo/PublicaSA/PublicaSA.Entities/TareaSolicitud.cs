namespace PublicaSA.Entities
{
    public class TareaSolicitud
    {
        public int IdTarea { get; set; }

        public int IdSolicitud { get; set; }

        public int IdUsuario { get; set; }

        public DateTime Fecha { get; set; }

        public string Descripcion { get; set; } = string.Empty;

        public decimal HorasInvertidas { get; set; }
    }
}