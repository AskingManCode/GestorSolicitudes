namespace PublicaSA.Entities
{
    public class UsuarioEditRequest
    {
        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; } = string.Empty!;
        public string Correo { get; set; } = string.Empty!;
        public string NombreUsuario { get; set; } = string.Empty!;
        public string CodigoEstado { get; set; } = string.Empty!;
        public string? ContrasenaActual { get; set; }
        public string? ContrasenaNueva { get; set; }
        public string? ConfirmarContrasena { get; set; }
    }
}