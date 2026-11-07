using Dapper;
using PublicaSA.Entities;
using PublicaSA.Repository.Interfaces;

namespace PublicaSA.Repository.Repositories
{
    public class ReporteMontosRepository : IReporteMontosRepository
    {
        private readonly IDBConnectionFactory _connectionFactory;

        public ReporteMontosRepository(
            IDBConnectionFactory connectionFactory)
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

            if (anio < 2000 || anio > 2150)
            {
                throw new ArgumentOutOfRangeException(nameof(anio));
            }

            // Une cada desglose mensual con los datos de su solicitud.
            const string sql = """
                SELECT s.ConsecutivoOficio,
                       s.Titulo,
                       d.Mes,
                       d.Anio,
                       d.Monto,
                       d.Iva,
                       d.Total
                FROM DesglosesSolicitud d
                INNER JOIN Solicitudes s
                    ON s.IdSolicitud = d.IdSolicitud
                WHERE d.Mes = @Mes
                  AND d.Anio = @Anio
                ORDER BY s.ConsecutivoOficio, s.IdSolicitud;
                """;

            using var connection = _connectionFactory.CreateConnection();

            // Los parámetros envían los valores separados del texto SQL.
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