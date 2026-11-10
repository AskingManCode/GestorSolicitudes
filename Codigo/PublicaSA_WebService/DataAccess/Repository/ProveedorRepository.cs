using Dapper;
using DataAccess.Infrastructure;
using Models;
using System.Data;

namespace DataAccess.Repository
{
    public class ProveedorRepository

    {
        public int Crear(
    Proveedor proveedor,
    IDbConnection conexion,
    IDbTransaction transaccion)
        {
            const string sql = @"
        INSERT INTO proveedores
            (CedulaJuridica, Nombre, NombreRepresentante,
             Telefono, Correo, Activo)
        VALUES
            (@CedulaJuridica, @Nombre, @NombreRepresentante,
             @Telefono, @Correo, 1);

        SELECT LAST_INSERT_ID();";

            // Envía únicamente los datos necesarios para crear al proveedor.
            var parametros = new
            {
                proveedor.CedulaJuridica,
                proveedor.Nombre,
                proveedor.NombreRepresentante,
                proveedor.Telefono,
                proveedor.Correo
            };

            // Usa la misma transacción que guardará la propuesta y sus desgloses.
            return conexion.ExecuteScalar<int>(
                sql,
                parametros,
                transaction: transaccion);
        }


        private readonly ConexionBD _conexionBD;

        public ProveedorRepository(ConexionBD conexionBD)
        {
            _conexionBD = conexionBD;
        }

        public Proveedor ObtenerPorCedula(string cedulaJuridica)
        {
            const string sql = @"
                SELECT IdProveedor, CedulaJuridica, Nombre,
                       NombreRepresentante, Telefono, Correo,
                       Activo, FechaCreacion
                FROM proveedores
                WHERE REPLACE(CedulaJuridica, '-', '') =
                      REPLACE(@CedulaJuridica, '-', '');";

            // La conexión se libera al terminar la consulta.
            using (var conexion = _conexionBD.CrearConexion())
            {
                return conexion.QuerySingleOrDefault<Proveedor>(
                    sql,
                    new { CedulaJuridica = cedulaJuridica });
            }
        }

        public Proveedor ObtenerPorCedula(
    string cedulaJuridica,
    IDbConnection conexion,
    IDbTransaction transaccion)
        {
            const string sql = @"
        SELECT IdProveedor, CedulaJuridica, Nombre,
               NombreRepresentante, Telefono, Correo,
               Activo, FechaCreacion
        FROM proveedores
        WHERE REPLACE(CedulaJuridica, '-', '') =
              REPLACE(@CedulaJuridica, '-', '');";

            // Consulta usando la conexión del registro completo.
            return conexion.QuerySingleOrDefault<Proveedor>(
                sql,
                new { CedulaJuridica = cedulaJuridica },
                transaction: transaccion);
        }
    }
}