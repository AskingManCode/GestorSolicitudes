namespace PublicaSA.Entities
{
    // Contiene únicamente los datos necesarios para seleccionar un usuario.
    public class UsuarioSeleccion
    {
        public int IdUsuario { get; set; }

        public string NombreCompleto { get; set; } = string.Empty;
    }
}