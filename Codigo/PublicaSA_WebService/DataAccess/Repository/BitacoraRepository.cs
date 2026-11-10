using Dapper;
using System.Data;

namespace DataAccess.Repository
{
    public class BitacoraRepository
    {
        public void Registrar(
            string usuarioEjecuta,
            int? idUsuario,
            string accion,
            string detalle,
            IDbConnection conexion,
            IDbTransaction transaccion)
        {
            const string sql = @"
                INSERT INTO bitacoras
                    (FechaBitacora, UsuarioEjecuta,
                     IdUsuario, Accion, Detalle)
                VALUES
                    (NOW(), @UsuarioEjecuta,
                     @IdUsuario, @Accion, @Detalle);";

            var parametros = new
            {
                UsuarioEjecuta = usuarioEjecuta,
                IdUsuario = idUsuario,
                Accion = accion,
                Detalle = detalle
            };

            // Guarda la auditoría junto con los datos de la propuesta.
            conexion.Execute(
                sql,
                parametros,
                transaction: transaccion);
        }
    }
}