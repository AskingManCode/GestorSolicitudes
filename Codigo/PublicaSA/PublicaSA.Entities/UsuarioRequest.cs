namespace PublicaSA.Entities
{
    public class UsuarioRequest
    {
        public int IdUsuario { get; set; } 
        public string NombreCompleto { get; set; } = string.Empty!;
        public string Correo { get; set; } = string.Empty!;
        public string NombreUsuario { get; set; } = string.Empty!;
        public string Contrasena { get; set; } = string.Empty!;
        public string CodigoEstado { get; set; } = string.Empty!;
    }
}

