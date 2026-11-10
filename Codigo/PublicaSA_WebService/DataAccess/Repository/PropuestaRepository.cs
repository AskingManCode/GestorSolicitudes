using Dapper;
using System.Data;

namespace DataAccess.Repository
{
    public class PropuestaRepository
    {
        public int Crear(
            int idSolicitud,
            int idProveedor,
            IDbConnection conexion,
            IDbTransaction transaccion)
        {
            const string sql = @"
                INSERT INTO propuestas
                    (IdSolicitud, IdProveedor, Aprobada, Activo)
                VALUES
                    (@IdSolicitud, @IdProveedor, 0, 1);

                SELECT LAST_INSERT_ID();";

            var parametros = new
            {
                IdSolicitud = idSolicitud,
                IdProveedor = idProveedor
            };

            // La propuesta queda pendiente de aprobación.
            // La base de datos asigna la fecha de registro.
            return conexion.ExecuteScalar<int>(
                sql,
                parametros,
                transaction: transaccion);
        }
        public bool ExisteParaSolicitud(
            int idSolicitud,
            int idProveedor,
            IDbConnection conexion,
             IDbTransaction transaccion)
        {
            const string sql = @"
                SELECT IdPropuesta
                FROM propuestas
                WHERE IdSolicitud = @IdSolicitud
                  AND IdProveedor = @IdProveedor;";

            var parametros = new
            {
                IdSolicitud = idSolicitud,
                IdProveedor = idProveedor
            };

            // También considera propuestas inactivas por la restricción de la tabla.
            int? idPropuesta = conexion.QuerySingleOrDefault<int?>(
                sql,
                parametros,
                transaction: transaccion);

            return idPropuesta.HasValue;
        }


    }
}