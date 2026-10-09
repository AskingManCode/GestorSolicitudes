using System;

namespace PublicaSA.Entities
{
    public class Solicitud
    {
        public int IdSolicitud { get; set; }
        public string ConsecutivoOficio { get; set; } = string.Empty;
        public string? DocumentoRespuesta { get; set; }
        public string? DocumentoInicio { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int IdRepresentante { get; set; }
        public string? Observaciones { get; set; }
        public int IdEstado { get; set; }
        public DateTime FechaIngreso { get; set; } = DateTime.Today;
        public DateTime? FechaRespuesta { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Propiedades de navegación / lectura
        public string? NombreRepresentante { get; set; }
        public string? EmailRepresentante { get; set; }
        public string? NombreEstado { get; set; }

        // Propiedad calculada para el estado de semáforo
        // "danger" (Vencida), "warning" (Vence hoy), "success" (Nueva), "default" (Otra)
        public string SemaforoColor
        {
            get
            {
                if (NombreEstado == "Vencida")
                    return "table-danger";

                if (FechaRespuesta.HasValue)
                {
                    if (NombreEstado == "Nueva")
                        return "table-success";
                    return "";
                }

                // Cálculo de 5 días hábiles a partir de FechaIngreso
                DateTime limite = CalcularQuintoDiaHabil(FechaIngreso);
                DateTime hoy = DateTime.Today;

                if (hoy > limite)
                {
                    return "table-danger";
                }
                else if (hoy == limite)
                {
                    return "table-warning";
                }
                else if (NombreEstado == "Nueva")
                {
                    return "table-success";
                }

                return "";
            }
        }

        public string EstadoSemaforoEtiqueta
        {
            get
            {
                if (NombreEstado == "Vencida")
                    return "Vencida";

                if (!FechaRespuesta.HasValue)
                {
                    DateTime limite = CalcularQuintoDiaHabil(FechaIngreso);
                    DateTime hoy = DateTime.Today;
                    if (hoy > limite) return "Vencida";
                    if (hoy == limite) return "Vence Hoy";
                }

                return NombreEstado ?? "Nueva";
            }
        }

        public static DateTime CalcularQuintoDiaHabil(DateTime fechaInicio)
        {
            DateTime actual = fechaInicio;
            int diasHabiles = 0;
            while (diasHabiles < 5)
            {
                actual = actual.AddDays(1);
                if (actual.DayOfWeek != DayOfWeek.Saturday && actual.DayOfWeek != DayOfWeek.Sunday)
                {
                    diasHabiles++;
                }
            }
            return actual.Date;
        }
    }
}
