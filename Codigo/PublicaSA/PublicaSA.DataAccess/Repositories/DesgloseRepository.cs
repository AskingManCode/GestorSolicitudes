using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using PublicaSA.DataAccess.Interfaces;
using PublicaSA.Entities;

namespace PublicaSA.DataAccess.Repositories
{
    public class DesgloseRepository : IDesgloseRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DesgloseRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<(IEnumerable<DesgloseSolicitud> Items, int TotalCount)> GetPagedBySolicitudAsync(int idSolicitud, int pageNumber, int pageSize)
        {
            using var connection = _connectionFactory.CreateConnection();
            int offset = (pageNumber - 1) * pageSize;

            const string sql = @"
                SELECT * FROM DesglosesSolicitud
                WHERE IdSolicitud = @IdSolicitud
                ORDER BY Anio DESC, Mes DESC, IdDesglose DESC
                LIMIT @Offset, @PageSize;

                SELECT COUNT(1) FROM DesglosesSolicitud WHERE IdSolicitud = @IdSolicitud;";

            using var multi = await connection.QueryMultipleAsync(sql, new { IdSolicitud = idSolicitud, Offset = offset, PageSize = pageSize });
            var items = await multi.ReadAsync<DesgloseSolicitud>();
            int total = await multi.ReadSingleAsync<int>();

            return (items, total);
        }

        public async Task<IEnumerable<DesgloseSolicitud>> GetBySolicitudIdAsync(int idSolicitud)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT * FROM DesglosesSolicitud
                WHERE IdSolicitud = @IdSolicitud
                ORDER BY Anio ASC, Mes ASC, IdDesglose ASC;";

            return await connection.QueryAsync<DesgloseSolicitud>(sql, new { IdSolicitud = idSolicitud });
        }

        public async Task<DesgloseSolicitud?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM DesglosesSolicitud WHERE IdDesglose = @Id;";
            return await connection.QueryFirstOrDefaultAsync<DesgloseSolicitud>(sql, new { Id = id });
        }

        public async Task<int> InsertAsync(DesgloseSolicitud desglose)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                INSERT INTO DesglosesSolicitud (
                    IdSolicitud, Mes, Anio, Horas, Monto,
                    Iva, Total, PorcentajeCobro, Observaciones, FechaCreacion
                )
                VALUES (
                    @IdSolicitud, @Mes, @Anio, @Horas, @Monto,
                    @Iva, @Total, @PorcentajeCobro, @Observaciones, NOW()
                );
                SELECT LAST_INSERT_ID();";

            return await connection.ExecuteScalarAsync<int>(sql, desglose);
        }

        public async Task<bool> UpdateAsync(DesgloseSolicitud desglose)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE DesglosesSolicitud
                SET Mes = @Mes,
                    Anio = @Anio,
                    Horas = @Horas,
                    Monto = @Monto,
                    Iva = @Iva,
                    Total = @Total,
                    PorcentajeCobro = @PorcentajeCobro,
                    Observaciones = @Observaciones
                WHERE IdDesglose = @IdDesglose;";

            int rows = await connection.ExecuteAsync(sql, desglose);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "DELETE FROM DesglosesSolicitud WHERE IdDesglose = @Id;";
            int rows = await connection.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }

        public async Task<bool> HasRelatedDataAsync(int id)
        {
            // En el modelo actual, los desgloses no tienen tablas hijas con FK, pero queda la estructura lista
            await Task.CompletedTask;
            return false;
        }
    }
}
