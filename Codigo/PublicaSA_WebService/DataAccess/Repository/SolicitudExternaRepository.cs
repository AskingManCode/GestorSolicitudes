using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using Dapper;
using Models;
using MySqlConnector;

namespace DataAccess.Repository
{
    public class SolicitudExternaRepository
    {
        private readonly string _connectionString;

        public SolicitudExternaRepository()
        {
            var config = ConfigurationManager.ConnectionStrings["GestorSolicitudesConnection"];
            _connectionString = config != null ? config.ConnectionString : "";
        }

        private IDbConnection ObtenerConexion()
        {
            return new MySqlConnection(_connectionString);
        }

        // HU EXT2: Lista de solicitudes que están publicadas y vigentes
        public List<SolicitudPublicadaDto> ObtenerSolicitudesPublicadas()
        {
            using (var db = ObtenerConexion())
            {
                string sql = @"
                    SELECT 
                        s.IdSolicitud,
                        s.ConsecutivoOficio,
                        s.Titulo,
                        r.Nombre AS NombreRepresentante
                    FROM Solicitudes s
                    LEFT JOIN Representantes r ON s.IdRepresentante = r.IdRepresentante
                    WHERE s.Activo = 1 
                      AND s.VisibleExterno = 1 
                      AND (s.FechaVencimientoPublicacion IS NULL OR s.FechaVencimientoPublicacion >= CURDATE())
                    ORDER BY s.FechaIngreso DESC;";

                return db.Query<SolicitudPublicadaDto>(sql).ToList();
            }
        }

        // HU EXT3: Detalle de una solicitud seleccionada
        public SolicitudDetalleDto ObtenerDetalleSolicitud(int idSolicitud)
        {
            using (var db = ObtenerConexion())
            {
                string sql = @"
                    SELECT 
                        s.IdSolicitud,
                        s.ConsecutivoOficio,
                        s.Titulo,
                        s.Descripcion,
                        s.DocumentoRespuesta,
                        s.DocumentoInicio,
                        r.Nombre AS NombreRepresentante,
                        s.Observaciones,
                        e.Nombre AS NombreEstado,
                        s.FechaIngreso,
                        s.FechaRespuesta,
                        s.FechaInicio,
                        s.FechaVencimientoPublicacion
                    FROM Solicitudes s
                    LEFT JOIN Representantes r ON s.IdRepresentante = r.IdRepresentante
                    LEFT JOIN EstadosSolicitud e ON s.CodigoEstado = e.CodigoEstado
                    WHERE s.IdSolicitud = @IdSolicitud AND s.Activo = 1;";

                return db.QueryFirstOrDefault<SolicitudDetalleDto>(sql, new { IdSolicitud = idSolicitud });
            }
        }
    }
}
