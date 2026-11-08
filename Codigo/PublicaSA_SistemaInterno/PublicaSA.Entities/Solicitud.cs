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
        public bool VisibleExterno { get; set; } = false;
        public DateTime? FechaVencimientoPublicacion { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Datos relacionados que se muestran en pantalla.
        public string? NombreRepresentante { get; set; }
        public string? EmailRepresentante { get; set; }
        public string? NombreEstado { get; set; }

        // Indica si la solicitud está visible en el módulo externo y su periodo de publicación sigue vigente.
        public bool EstaVisibleExternoVigente
        {
            get
            {
                if (!VisibleExterno || !FechaVencimientoPublicacion.HasValue)
                    return false;

                // Usa la fecha de Costa Rica (UTC-6)
                DateTime hoy = DateTimeOffset.UtcNow
                    .ToOffset(TimeSpan.FromHours(-6)).Date;

                return FechaVencimientoPublicacion.Value.Date >= hoy;
            }
        }

        // Descripción textual del estado de publicación externa.
        public string EstadoPublicacionTexto
        {
            get
            {
                if (!VisibleExterno)
                    return "No publicada";

                if (EstaVisibleExternoVigente)
                    return $"Publicada (vence {FechaVencimientoPublicacion:dd/MM/yyyy})";

                return $"Vencida ({FechaVencimientoPublicacion:dd/MM/yyyy})";
            }
        }

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
