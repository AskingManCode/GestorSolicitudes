using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using PublicaSA.Entities;
using PublicaSA.Repository.Interfaces;

namespace PublicaSA.Repository.Repositories
{
    public class BitacoraRepository : IBitacoraRepository
    {
        private readonly IDBConnectionFactory _connectionFactory;

        public BitacoraRepository(IDBConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> InsertAsync(Bitacora bitacora)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                INSERT INTO Bitacoras (FechaBitacora, UsuarioEjecuta, IdUsuario, Accion, Detalle)
                VALUES (NOW(), @UsuarioEjecuta, @IdUsuario, @Accion, @Detalle);
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
