namespace Models
{
    public class ResultadoRegistroPropuesta
    {
        // Indica si el registro se completó.
        public bool Exitoso { get; set; }

        // Mensaje que se mostrará al usuario.
        public string Mensaje { get; set; }

        // Solo tendrá valor cuando la propuesta se haya guardado.
        public int? IdPropuesta { get; set; }
    }
}