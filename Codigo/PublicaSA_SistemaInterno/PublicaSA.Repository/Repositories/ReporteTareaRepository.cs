using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using PublicaSA.Repository.Interfaces;
using PublicaSA.Entities;

namespace PublicaSA.Repository.Repositories
{
    public class ReporteTareaRepository : IReporteTareaRepository
    {
        private readonly IDBConnectionFactory _connectionFactory;

        public ReporteTareaRepository(IDBConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Usuario>> GetUsuariosAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT IdUsuario, NombreUsuario, NombreCompleto FROM Usuarios ORDER BY NombreCompleto ASC;";
            return await connection.QueryAsync<Usuario>(sql);
        }

        public async Task<IEnumerable<ReporteTarea>> GetTareasPorUsuarioAsync(int idUsuario, int mes, int anio)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT u.NombreCompleto AS NombreUsuario, t.Fecha, t.Descripcion, t.HorasInvertidas,
                       s.ConsecutivoOficio, s.Titulo
                FROM Tareas t
                INNER JOIN Usuarios u ON t.IdUsuario = u.IdUsuario
                INNER JOIN Solicitudes s ON t.IdSolicitud = s.IdSolicitud
                WHERE t.IdUsuario = @IdUsuario
                  AND MONTH(t.Fecha) = @Mes
                  AND YEAR(t.Fecha) = @Anio
                ORDER BY t.Fecha ASC, s.ConsecutivoOficio ASC;";

            return await connection.QueryAsync<ReporteTarea>(sql, new { IdUsuario = idUsuario, Mes = mes, Anio = anio });
        }

        public async Task<IEnumerable<Solicitud>> GetSolicitudesAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT IdSolicitud, ConsecutivoOficio, Titulo FROM Solicitudes ORDER BY ConsecutivoOficio ASC;";
            return await connection.QueryAsync<Solicitud>(sql);
        }

        public async Task<IEnumerable<ReporteTarea>> GetTareasPorSolicitudAsync(int idSolicitud, int mes, int anio)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT u.NombreCompleto AS NombreUsuario, t.Fecha, t.Descripcion, t.HorasInvertidas,
                       s.ConsecutivoOficio, s.Titulo
                FROM Tareas t
                INNER JOIN Usuarios u ON t.IdUsuario = u.IdUsuario
                INNER JOIN Solicitudes s ON t.IdSolicitud = s.IdSolicitud
                WHERE t.IdSolicitud = @IdSolicitud
                  AND MONTH(t.Fecha) = @Mes
                  AND YEAR(t.Fecha) = @Anio
                ORDER BY t.Fecha ASC, u.NombreCompleto ASC;";

            return await connection.QueryAsync<ReporteTarea>(sql, new { IdSolicitud = idSolicitud, Mes = mes, Anio = anio });
        }
    }
}
