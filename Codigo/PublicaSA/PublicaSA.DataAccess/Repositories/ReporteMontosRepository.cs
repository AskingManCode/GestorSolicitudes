using Dapper;
using PublicaSA.DataAccess.Interfaces;
using PublicaSA.Entities;

namespace PublicaSA.DataAccess.Repositories
{
    public class ReporteMontosRepository : IReporteMontosRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ReporteMontosRepository(
            IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<MontoSolicitudMensual>>
            ListarPorMesAsync(int mes, int anio)
        {
            if (mes < 1 || mes > 12)
            {
                throw new ArgumentOutOfRangeException(nameof(mes));
            }

            if (anio < 1 || anio > 65535)
            {
                throw new ArgumentOutOfRangeException(nameof(anio));
            }

            // Une cada desglose mensual con los datos de su solicitud.
            const string sql = """
                SELECT
                    s.ConsecutivoOficio,
                    s.Titulo,
                    d.Mes,
                    d.Anio,
                    d.Monto,
                    d.Iva,
                    d.Total
                FROM DesglosesSolicitud AS d
                INNER JOIN Solicitudes AS s
                    ON s.IdSolicitud = d.IdSolicitud
                WHERE d.Mes = @Mes
                  AND d.Anio = @Anio
                ORDER BY s.ConsecutivoOficio;
                """;

            using var connection = _connectionFactory.CreateConnection();

            // Los parámetros se envían separados del texto de la consulta.
            return await connection.QueryAsync<MontoSolicitudMensual>(
                sql,
                new
                {
                    Mes = mes,
                    Anio = anio
                });
        }
    }
}