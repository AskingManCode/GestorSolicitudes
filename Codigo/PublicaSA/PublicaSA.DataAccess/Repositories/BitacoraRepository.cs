using Dapper;
using PublicaSA.DataAccess.Interfaces;

namespace PublicaSA.DataAccess.Repositories
{
    public class BitacoraRepository : IBitacoraRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public BitacoraRepository(
            IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task RegistrarAsync(
            int? idUsuario,
            string tipoAccion,
            string descripcion)
        {
            const string sql = """
                INSERT INTO Bitacoras
                    (IdUsuario, TipoAccion, Descripcion)
                VALUES
                    (@IdUsuario, @TipoAccion, @Descripcion);
                """;

            using var connection = _connectionFactory.CreateConnection();

            await connection.ExecuteAsync(
                sql,
                new
                {
                    IdUsuario = idUsuario,
                    TipoAccion = tipoAccion,
                    Descripcion = descripcion
                });
        }
    }
}