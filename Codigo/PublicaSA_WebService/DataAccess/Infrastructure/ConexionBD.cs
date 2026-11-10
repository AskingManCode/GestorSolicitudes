using MySqlConnector;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DataAccess.Infrastructure
{
    public class ConexionBD
    {
        private readonly string _cadenaConexion;

        public ConexionBD(string cadenaConexion)
        {
            // Evita crear conexiones sin una configuración válida.
            if (string.IsNullOrWhiteSpace(cadenaConexion))
            {
                throw new ArgumentException(
                    "Debe configurar la conexión a la base de datos.",
                    nameof(cadenaConexion));
            }

            _cadenaConexion = cadenaConexion;
        }

        public IDbConnection CrearConexion()
        {
            // Cada operación recibe su propia conexión.
            return new MySqlConnection(_cadenaConexion);
        }
    }
}