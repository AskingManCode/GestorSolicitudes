using System.Data;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using PublicaSA.DataAccess.Interfaces;

namespace PublicaSA.DataAccess.Repositories
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(IConfiguration configuration)
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
