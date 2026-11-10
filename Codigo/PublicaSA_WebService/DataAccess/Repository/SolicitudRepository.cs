using Dapper;
using System;
using System.Data;

namespace DataAccess.Repository
{
    public class SolicitudRepository
    {
        public bool EstaDisponibleParaParticipar(
            int idSolicitud,
            DateTime fechaActual,
            IDbConnection conexion,
            IDbTransaction transaccion)
        {
            const string sql = @"
                SELECT IdSolicitud
                FROM solicitudes
                WHERE IdSolicitud = @IdSolicitud
                  AND Activo = 1
                  AND VisibleExterno = 1
                  AND FechaVencimientoPublicacion >= @FechaActual
                FOR UPDATE;";

            // La fecha llegará desde el servicio, sin incluir la hora.
            var parametros = new
            {
                IdSolicitud = idSolicitud,
                FechaActual = fechaActual.Date
            };

            // Mantiene bloqueada la solicitud hasta terminar la transacción.
            int? resultado = conexion.QuerySingleOrDefault<int?>(
                sql,
                parametros,
                transaction: transaccion);

            return resultado.HasValue;
        }
    }
}