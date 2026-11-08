using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using PublicaSA.Entities;
using PublicaSA.Repository.Interfaces;

namespace PublicaSA.Repository.Repositories
{
    public class PropuestaRepository : IPropuestaRepository
    {
        private readonly IDBConnectionFactory _connectionFactory;

        public PropuestaRepository(IDBConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Propuesta>> ListarPorSolicitudAsync(int idSolicitud)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Propuesta>(
                "sp_Propuestas_ListarPorSolicitud",
                new { p_IdSolicitud = idSolicitud },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<(Propuesta? Propuesta, IEnumerable<DesglosePropuesta> Desgloses)> ObtenerDetalleAsync(int idPropuesta)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var multi = await connection.QueryMultipleAsync(
                "sp_Propuestas_ObtenerDetalle",
                new { p_IdPropuesta = idPropuesta },
                commandType: CommandType.StoredProcedure);

            var propuesta = await multi.ReadFirstOrDefaultAsync<Propuesta>();
            var desgloses = propuesta != null
                ? await multi.ReadAsync<DesglosePropuesta>()
                : Enumerable.Empty<DesglosePropuesta>();

            return (propuesta, desgloses);
        }

        public async Task<Propuesta?> AprobarAsync(int idPropuesta, int idSolicitud)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Propuesta>(
                "sp_Propuestas_Aprobar",
                new
                {
                    p_IdPropuesta = idPropuesta,
                    p_IdSolicitud = idSolicitud
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}
