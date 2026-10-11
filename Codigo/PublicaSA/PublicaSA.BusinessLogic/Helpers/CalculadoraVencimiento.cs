namespace PublicaSA.BusinessLogic.Helpers
{
    public static class CalculadoraVencimiento
    {
        public static DateTime CalcularFechaVencimiento(
            DateTime fechaIngreso)
        {
            DateTime fecha = fechaIngreso.Date;
            int diasHabiles = 0;

            // Cuenta desde el día siguiente al ingreso.
            while (diasHabiles < 5)
            {
                fecha = fecha.AddDays(1);

                // Los sábados y domingos no cuentan como días hábiles.
                if (fecha.DayOfWeek != DayOfWeek.Saturday &&
                    fecha.DayOfWeek != DayOfWeek.Sunday)
                {
                    diasHabiles++;
                }
            }

            return fecha;
        }
    }
}