using Dapper;
using Models;
using System.Data;

namespace DataAccess.Repository
{
    public class DesglosePropuestaRepository
    {
        public int Crear(
            int idPropuesta,
            DesglosePropuesta desglose,
            IDbConnection conexion,
            IDbTransaction transaccion)
        {
            const string sql = @"
                INSERT INTO desglosespropuestas
                    (IdPropuesta, Mes, Anio, Horas, Monto,
                     Iva, Total, PorcentajeCobro,
                     Observaciones, Activo)
                VALUES
                    (@IdPropuesta, @Mes, @Anio, @Horas, @Monto,
                     @Iva, @Total, @PorcentajeCobro,
                     @Observaciones, 1);

                SELECT LAST_INSERT_ID();";

            // Relaciona la línea con la propuesta recién creada.
            var parametros = new
            {
                IdPropuesta = idPropuesta,
                desglose.Mes,
                desglose.Anio,
                desglose.Horas,
                desglose.Monto,
                desglose.Iva,
                desglose.Total,
                desglose.PorcentajeCobro,
                desglose.Observaciones
            };

            // Mantiene el desglose dentro de la misma transacción.
            return conexion.ExecuteScalar<int>(
                sql,
                parametros,
                transaction: transaccion);
        }
    }
}