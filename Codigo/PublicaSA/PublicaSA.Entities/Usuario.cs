namespace PublicaSA.Entities
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = string.Empty!;
        public string NombreCompleto { get; set; } = string.Empty!;
        public string Correo { get; set; } = string.Empty!;
        public string Contrasena { get; set; } = string.Empty!;
        public string CodigoEstado { get; set; } = string.Empty!;
        public int IntentosFallidos { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}