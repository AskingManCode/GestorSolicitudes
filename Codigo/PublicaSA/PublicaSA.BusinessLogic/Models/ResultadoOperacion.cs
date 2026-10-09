namespace PublicaSA.BusinessLogic.Models
{
    public class ResultadoOperacion
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public object? Datos { get; set; }

        public static ResultadoOperacion Exito(string mensaje = "Operación realizada con éxito.", object? datos = null)
        {
            return new ResultadoOperacion { Exitoso = true, Mensaje = mensaje, Datos = datos };
        }

        public static ResultadoOperacion Error(string mensaje, object? datos = null)
        {
            return new ResultadoOperacion { Exitoso = false, Mensaje = mensaje, Datos = datos };
        }
    }
}
