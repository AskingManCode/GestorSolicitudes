using PublicaSA.BusinessLogic.Interfaces;
using PublicaSA.DataAccess.Interfaces;
using PublicaSA.Entities;
using System.Text.Json;

namespace PublicaSA.BusinessLogic.Services
{
    public class TareaSolicitudService : ITareaSolicitudService
    {
        private readonly ITareaSolicitudRepository _tareas;
        private readonly IBitacoraRepository _bitacoras;

        public TareaSolicitudService(
            ITareaSolicitudRepository tareas,
            IBitacoraRepository bitacoras)
        {
            _tareas = tareas;
            _bitacoras = bitacoras;
        }

        // Obtiene los usuarios y registra quién realizó la consulta.
        public async Task<IEnumerable<UsuarioSeleccion>> ListarUsuariosAsync(
            int idUsuarioActual)
        {
            if (idUsuarioActual <= 0)
            {
                throw new ArgumentException(
                    "Se requiere un usuario autenticado.",
                    nameof(idUsuarioActual));
            }

            try
            {
                var usuarios = await _tareas.ListarUsuariosAsync();

                await _bitacoras.RegistrarAsync(
                    idUsuarioActual,
                    "CONSULTA",
                    "El usuario consulta usuarios");

                return usuarios;
            }
            catch (Exception error)
            {
                try
                {
                    await _bitacoras.RegistrarAsync(
                        idUsuarioActual,
                        "ERROR",
                        "Error al consultar usuarios. "
                        + JsonSerializer.Serialize(new
                        {
                            TipoError = error.GetType().Name
                        }));
                }
                catch
                {
                    // Conserva el error original si también falla la bitácora.
                }

                throw;
            }
        }


        // Consulta las tareas y registra quién solicitó el listado.
        public async Task<(IEnumerable<TareaSolicitud> Tareas, int Total)>
            ListarPorSolicitudAsync(
                int idSolicitud,
                int pagina,
                int idUsuarioActual)
        {
            if (idUsuarioActual <= 0)
            {
                throw new ArgumentException(
                    "Se requiere un usuario autenticado.",
                    nameof(idUsuarioActual));
            }

            try
            {
                var tareas = await _tareas.ListarPorSolicitudAsync(
                    idSolicitud,
                    pagina);

                var total = await _tareas.ContarPorSolicitudAsync(
                    idSolicitud);

                await _bitacoras.RegistrarAsync(
                    idUsuarioActual,
                    "CONSULTA",
                    "El usuario consulta tareas de la solicitud "
                    + idSolicitud);

                return (tareas, total);
            }
            catch (Exception error)
            {
                try
                {
                    await _bitacoras.RegistrarAsync(
                        idUsuarioActual,
                        "ERROR",
                        "Error al consultar tareas. "
                        + JsonSerializer.Serialize(new
                        {
                            IdSolicitud = idSolicitud,
                            Pagina = pagina,
                            TipoError = error.GetType().Name
                        }));
                }
                catch
                {
                    // Conserva el error original si la bitácora falla.
                }

                throw;
            }
        }

        // Delega la creación y su bitácora al repositorio.
        public async Task<int> CrearAsync(
            TareaSolicitud tarea,
            int idUsuarioActual)
        {
            ArgumentNullException.ThrowIfNull(tarea);

            if (idUsuarioActual <= 0)
            {
                throw new ArgumentException(
                    "Se requiere un usuario autenticado.",
                    nameof(idUsuarioActual));
            }

            try
            {
                return await _tareas.CrearAsync(
                    tarea,
                    idUsuarioActual);
            }
            catch (Exception error)
            {
                try
                {
                    await _bitacoras.RegistrarAsync(
                        idUsuarioActual,
                        "ERROR",
                        "Error al crear una tarea. "
                        + JsonSerializer.Serialize(new
                        {
                            tarea.IdSolicitud,
                            TipoError = error.GetType().Name
                        }));
                }
                catch
                {
                    // Conserva el error original si la bitácora falla.
                }

                throw;
            }
        }

        // Obtiene una tarea de la solicitud y registra la consulta.
        public async Task<TareaSolicitud?> ObtenerPorIdAsync(
            int idTarea,
            int idSolicitud,
            int idUsuarioActual)
        {
            if (idUsuarioActual <= 0)
            {
                throw new ArgumentException(
                    "Se requiere un usuario autenticado.",
                    nameof(idUsuarioActual));
            }

            try
            {
                var tarea = await _tareas.ObtenerPorIdAsync(
                    idTarea,
                    idSolicitud);

                await _bitacoras.RegistrarAsync(
                    idUsuarioActual,
                    "CONSULTA",
                    "El usuario consulta tarea " + idTarea
                    + " de la solicitud " + idSolicitud);

                return tarea;
            }
            catch (Exception error)
            {
                try
                {
                    await _bitacoras.RegistrarAsync(
                        idUsuarioActual,
                        "ERROR",
                        "Error al consultar una tarea. "
                        + JsonSerializer.Serialize(new
                        {
                            IdTarea = idTarea,
                            IdSolicitud = idSolicitud,
                            TipoError = error.GetType().Name
                        }));
                }
                catch
                {
                    // Conserva el error original si la bitácora falla.
                }

                throw;
            }
        }

        // Delega la actualización y el registro de los datos anteriores y actuales.
        public async Task<bool> ActualizarAsync(
            TareaSolicitud tarea,
            int idUsuarioActual)
        {
            ArgumentNullException.ThrowIfNull(tarea);

            if (idUsuarioActual <= 0)
            {
                throw new ArgumentException(
                    "Se requiere un usuario autenticado.",
                    nameof(idUsuarioActual));
            }

            try
            {
                return await _tareas.ActualizarAsync(
                    tarea,
                    idUsuarioActual);
            }
            catch (Exception error)
            {
                try
                {
                    await _bitacoras.RegistrarAsync(
                        idUsuarioActual,
                        "ERROR",
                        "Error al actualizar una tarea. "
                        + JsonSerializer.Serialize(new
                        {
                            tarea.IdTarea,
                            tarea.IdSolicitud,
                            TipoError = error.GetType().Name
                        }));
                }
                catch
                {
                    // Conserva el error original si la bitácora falla.
                }

                throw;
            }
        }

        // Delega la eliminación y su bitácora al repositorio.
        public async Task<bool> EliminarAsync(
            int idTarea,
            int idSolicitud,
            int idUsuarioActual)
        {
            if (idUsuarioActual <= 0)
            {
                throw new ArgumentException(
                    "Se requiere un usuario autenticado.",
                    nameof(idUsuarioActual));
            }

            try
            {
                return await _tareas.EliminarAsync(
                    idTarea,
                    idSolicitud,
                    idUsuarioActual);
            }
            catch (Exception error)
            {
                try
                {
                    await _bitacoras.RegistrarAsync(
                        idUsuarioActual,
                        "ERROR",
                        "Error al eliminar una tarea. "
                        + JsonSerializer.Serialize(new
                        {
                            IdTarea = idTarea,
                            IdSolicitud = idSolicitud,
                            TipoError = error.GetType().Name
                        }));
                }
                catch
                {
                    // Conserva el error original si la bitácora falla.
                }

                throw;
            }
        }
    }
}