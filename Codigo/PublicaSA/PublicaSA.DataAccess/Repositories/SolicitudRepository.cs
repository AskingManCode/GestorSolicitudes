using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using PublicaSA.DataAccess.Interfaces;
using PublicaSA.Entities;

namespace PublicaSA.DataAccess.Repositories
{
    public class SolicitudRepository : ISolicitudRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public SolicitudRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<(IEnumerable<Solicitud> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
        {
            using var connection = _connectionFactory.CreateConnection();
            int offset = (pageNumber - 1) * pageSize;

            const string sql = @"
                SELECT s.*, r.Nombre AS NombreRepresentante, r.Email AS EmailRepresentante, e.Nombre AS NombreEstado
                FROM Solicitudes s
                INNER JOIN Representantes r ON s.IdRepresentante = r.IdRepresentante
                INNER JOIN EstadosSolicitud e ON s.IdEstado = e.IdEstado
                ORDER BY s.IdSolicitud DESC
                LIMIT @Offset, @PageSize;

                SELECT COUNT(1) FROM Solicitudes;";

            using var multi = await connection.QueryMultipleAsync(sql, new { Offset = offset, PageSize = pageSize });
            var items = await multi.ReadAsync<Solicitud>();
            int total = await multi.ReadSingleAsync<int>();

            return (items, total);
        }

        public async Task<Solicitud?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT s.*, r.Nombre AS NombreRepresentante, r.Email AS EmailRepresentante, e.Nombre AS NombreEstado
                FROM Solicitudes s
                INNER JOIN Representantes r ON s.IdRepresentante = r.IdRepresentante
                INNER JOIN EstadosSolicitud e ON s.IdEstado = e.IdEstado
                WHERE s.IdSolicitud = @Id;";

            return await connection.QueryFirstOrDefaultAsync<Solicitud>(sql, new { Id = id });
        }

        public async Task<Solicitud?> GetByConsecutivoAsync(string consecutivo)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT s.*, r.Nombre AS NombreRepresentante, r.Email AS EmailRepresentante, e.Nombre AS NombreEstado
                FROM Solicitudes s
                INNER JOIN Representantes r ON s.IdRepresentante = r.IdRepresentante
                INNER JOIN EstadosSolicitud e ON s.IdEstado = e.IdEstado
                WHERE s.ConsecutivoOficio = @Consecutivo;";

            return await connection.QueryFirstOrDefaultAsync<Solicitud>(sql, new { Consecutivo = consecutivo });
        }

        public async Task<int> InsertAsync(Solicitud solicitud)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                INSERT INTO Solicitudes (
                    ConsecutivoOficio, DocumentoRespuesta, DocumentoInicio,
                    Titulo, Descripcion, IdRepresentante, Observaciones,
                    IdEstado, FechaIngreso, FechaRespuesta, FechaInicio, FechaCreacion
                )
                VALUES (
                    @ConsecutivoOficio, @DocumentoRespuesta, @DocumentoInicio,
                    @Titulo, @Descripcion, @IdRepresentante, @Observaciones,
                    @IdEstado, @FechaIngreso, @FechaRespuesta, @FechaInicio, NOW()
                );
                SELECT LAST_INSERT_ID();";

            return await connection.ExecuteScalarAsync<int>(sql, solicitud);
        }

        public async Task<bool> UpdateAsync(Solicitud solicitud)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE Solicitudes
                SET ConsecutivoOficio = @ConsecutivoOficio,
                    DocumentoRespuesta = @DocumentoRespuesta,
                    DocumentoInicio = @DocumentoInicio,
                    Titulo = @Titulo,
                    Descripcion = @Descripcion,
                    IdRepresentante = @IdRepresentante,
                    Observaciones = @Observaciones,
                    IdEstado = @IdEstado,
                    FechaIngreso = @FechaIngreso,
                    FechaRespuesta = @FechaRespuesta,
                    FechaInicio = @FechaInicio
                WHERE IdSolicitud = @IdSolicitud;";

            int rows = await connection.ExecuteAsync(sql, solicitud);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "DELETE FROM Solicitudes WHERE IdSolicitud = @Id;";
            int rows = await connection.ExecuteAsync(sql, new { Id = id });
            return rows > 0;
        }

        public async Task<bool> HasRelatedDataAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT 
                    (SELECT COUNT(1) FROM DesglosesSolicitud WHERE IdSolicitud = @Id) +
                    (SELECT COUNT(1) FROM Tareas WHERE IdSolicitud = @Id);";

            int count = await connection.ExecuteScalarAsync<int>(sql, new { Id = id });
            return count > 0;
        }
    }
}
