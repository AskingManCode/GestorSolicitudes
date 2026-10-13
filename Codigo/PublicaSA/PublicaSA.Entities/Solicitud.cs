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
        public string CodigoEstado { get; set; } = "NUEVA";
        public DateTime FechaIngreso { get; set; } = DateTime.Today;
        public DateTime? FechaRespuesta { get; set; }
        public DateTime? FechaInicio { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Datos relacionados que se muestran en pantalla.
        public string? NombreRepresentante { get; set; }
        public string? EmailRepresentante { get; set; }
        public string? NombreEstado { get; set; }

        // Determina el color del semáforo.
        public string SemaforoColor
        {
            get
            {
                if (NombreEstado == "Vencida" || CodigoEstado == "VENCIDA")
                    return "table-danger";

                if (FechaRespuesta.HasValue)
                {
                    if (NombreEstado == "Nueva" || CodigoEstado == "NUEVA")
                        return "table-success";

                    return "";
                }

                DateTime limite = CalcularQuintoDiaHabil(FechaIngreso);

                // Usa la fecha de Costa Rica, igual que SOL5.
                DateTime hoy = DateTimeOffset.UtcNow
                    .ToOffset(TimeSpan.FromHours(-6)).Date;

                if (hoy > limite)
                {
                    return "table-danger";
                }
                else if (hoy == limite)
                {
                    return "table-warning";
                }
                else if (NombreEstado == "Nueva" || CodigoEstado == "NUEVA")
                {
                    return "table-success";
                }

                return "";
            }
        }

        // Determina el texto del semáforo.
        public string EstadoSemaforoEtiqueta
        {
            get
            {
                if (NombreEstado == "Vencida" || CodigoEstado == "VENCIDA")
                    return "Vencida";

                if (!FechaRespuesta.HasValue)
                {
                    DateTime limite = CalcularQuintoDiaHabil(FechaIngreso);

                    // Usa la misma fecha que el cálculo del color.
                    DateTime hoy = DateTimeOffset.UtcNow
                        .ToOffset(TimeSpan.FromHours(-6)).Date;

                    if (hoy > limite)
                        return "Vencida";

                    if (hoy == limite)
                        return "Vence Hoy";
                }

                return NombreEstado ?? "Nueva";
            }
        }

        // Cuenta cinco días desde el día siguiente al ingreso,
        // excluyendo sábados y domingos.
        public static DateTime CalcularQuintoDiaHabil(DateTime fechaInicio)
        {
            DateTime actual = fechaInicio;
            int diasHabiles = 0;

            while (diasHabiles < 5)
            {
                actual = actual.AddDays(1);

                if (actual.DayOfWeek != DayOfWeek.Saturday
                    && actual.DayOfWeek != DayOfWeek.Sunday)
                {
                    diasHabiles++;
                }
            }

            return actual.Date;
        }
    }
}