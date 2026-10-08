using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using PublicaSA.DataAccess.Interfaces;
using PublicaSA.Entities;

namespace PublicaSA.DataAccess.Repositories
{
    public class BitacoraRepository : IBitacoraRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public BitacoraRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> InsertAsync(Bitacora bitacora)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                INSERT INTO Bitacoras (FechaBitacora, UsuarioEjecuta, Accion, Detalle)
                VALUES (NOW(), @UsuarioEjecuta, @Accion, @Detalle);
                SELECT LAST_INSERT_ID();";

            return await connection.ExecuteScalarAsync<int>(sql, bitacora);
        }

        public async Task<IEnumerable<Bitacora>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM Bitacoras ORDER BY IdBitacora DESC;";
            return await connection.QueryAsync<Bitacora>(sql);
        }
    }
}
