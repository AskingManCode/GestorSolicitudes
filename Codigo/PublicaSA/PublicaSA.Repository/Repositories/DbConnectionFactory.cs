using PublicaSA.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using MySqlConnector; 
using System.Data;

namespace PublicaSA.Repository.Repositories 
{
    public class DBConnectionFactory : IDBConnectionFactory
    {
        private readonly string _connectionString;

        public DBConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Cadena de conexión 'DefaultConnection' no encontrada en la configuración.");
        }

        public IDbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
