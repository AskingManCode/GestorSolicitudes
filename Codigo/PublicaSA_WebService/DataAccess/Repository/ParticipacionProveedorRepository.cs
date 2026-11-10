using System.Collections.Generic;
using System.Linq;
using Dapper;
using DataAccess.Infrastructure;
using Models;

namespace DataAccess.Repository
{
    // HU EXT6: consulta las solicitudes en las que participó un proveedor.
    // Los nombres de tabla respetan mayúsculas (Proveedores, Propuestas,
    // Bitacoras) para funcionar también en servidores MySQL sensibles a mayúsculas.
    public class ParticipacionProveedorRepository
    {
        private readonly ConexionBD _conexionBD;

        public ParticipacionProveedorRepository(ConexionBD conexionBD)
        {
            _conexionBD = conexionBD;
        }

        // Acepta la cédula con o sin guiones (3-101-123456 o 3101123456).
        public Proveedor ObtenerProveedorPorCedula(string cedulaJuridica)
        {
            const string sql = @"
                SELECT IdProveedor, CedulaJuridica, Nombre,
                       NombreRepresentante, Telefono, Correo,
                       Activo, FechaCreacion
                FROM Proveedores
                WHERE REPLACE(CedulaJuridica, '-', '') =
                      REPLACE(@CedulaJuridica, '-', '');";

            using (var conexion = _conexionBD.CrearConexion())
            {
                return conexion.QueryFirstOrDefault<Proveedor>(
                    sql,
                    new { CedulaJuridica = cedulaJuridica });
            }
        }

        public List<ParticipacionProveedorDto> ListarPorProveedor(int idProveedor)
        {
            const string sql = @"
                SELECT s.IdSolicitud,
                       s.ConsecutivoOficio,
                       s.Titulo
                FROM Propuestas p
                INNER JOIN Solicitudes s ON s.IdSolicitud = p.IdSolicitud
                WHERE p.IdProveedor = @IdProveedor
                  AND p.Activo = 1
                GROUP BY s.IdSolicitud, s.ConsecutivoOficio, s.Titulo
                ORDER BY MAX(p.FechaPropuesta) DESC;";

            using (var conexion = _conexionBD.CrearConexion())
            {
                return conexion.Query<ParticipacionProveedorDto>(
                    sql,
                    new { IdProveedor = idProveedor }).ToList();
            }
        }

        // Bitácora de consultas: "El usuario consulta <elemento>".
        public void RegistrarConsulta(string cedulaJuridica)
        {
            const string sql = @"
                INSERT INTO Bitacoras
                    (FechaBitacora, UsuarioEjecuta, IdUsuario, Accion, Detalle)
                VALUES
                    (NOW(), 'Proveedor externo', NULL, 'CONSULTAR', @Detalle);";

            using (var conexion = _conexionBD.CrearConexion())
            {
                conexion.Execute(sql, new
                {
                    Detalle = "El usuario consulta participaciones en solicitudes. " +
                              "Cédula jurídica: " + cedulaJuridica
                });
            }
        }
    }
}
