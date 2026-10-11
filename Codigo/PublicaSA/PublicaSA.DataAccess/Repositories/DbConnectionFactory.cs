using System.Data;
using MySqlConnector;
using PublicaSA.DataAccess.Interfaces;

namespace PublicaSA.DataAccess.Repositories
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException(
                    "La cadena de conexión no está configurada.",
                    nameof(connectionString));
            }

            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}