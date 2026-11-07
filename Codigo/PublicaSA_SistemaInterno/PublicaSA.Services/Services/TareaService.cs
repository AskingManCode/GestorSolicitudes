using PublicaSA.Entities;
using PublicaSA.Repository.Interfaces;
using PublicaSA.Services.Interfaces;

namespace PublicaSA.Services.Services
{
    public class TareaService : ITareaService
    {
        private readonly ITareaRepository _tareas;
        private readonly IBitacoraService _bitacoras;

        public TareaService(
            ITareaRepository tareas,
            IBitacoraService bitacoras)
        {
            _tareas = tareas;
            _bitacoras = bitacoras;
        }

        // Obtiene los usuarios y registra la consulta en la bitácora.
        public async Task<IEnumerable<Usuario>> ListarUsuariosAsync(
            string usuarioActual)
        {
            ValidarUsuarioActual(usuarioActual);

            try
            {
                var usuarios = await _tareas.ListarUsuariosAsync();

                await _bitacoras.RegistrarAccionAsync(
                    usuarioActual,
                    "CONSULTA",
                    "El usuario consulta usuarios");

                return usuarios;
            }
            catch (Exception error)
            {
                await RegistrarErrorAsync(
                    usuarioActual, "consultar usuarios", error);
                throw;
            }
        }

        // Consulta las tareas y registra quién solicitó el listado.
        public async Task<(IEnumerable<Tarea> Tareas, int Total)>
            ListarPorSolicitudAsync(
                int idSolicitud,
                int pagina,
                string usuarioActual)
        {
            ValidarUsuarioActual(usuarioActual);

            try
            {
                ValidarIdentificador(idSolicitud, nameof(idSolicitud));

                if (pagina < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(pagina));
                }

                var tareas = await _tareas.ListarPorSolicitudAsync(
                    idSolicitud, pagina);

                int total = await _tareas.ContarPorSolicitudAsync(
                    idSolicitud);

                await _bitacoras.RegistrarAccionAsync(
                    usuarioActual,
                    "CONSULTA",
                    "El usuario consulta tareas de la solicitud "
                        + idSolicitud);

                return (tareas, total);
            }
            catch (Exception error)
            {
                await RegistrarErrorAsync(
                    usuarioActual, "consultar tareas", error);
                throw;
            }
        }

        public async Task<Tarea?> ObtenerPorIdAsync(
            int idTarea,
            int idSolicitud,
            string usuarioActual)
        {
            ValidarUsuarioActual(usuarioActual);

            try
            {
                ValidarIdentificador(idTarea, nameof(idTarea));
                ValidarIdentificador(idSolicitud, nameof(idSolicitud));

                var tarea = await _tareas.ObtenerPorIdAsync(
                    idTarea, idSolicitud);

                await _bitacoras.RegistrarAccionAsync(
                    usuarioActual,
                    "CONSULTA",
                    "El usuario consulta tarea " + idTarea
                        + " de la solicitud " + idSolicitud);

                return tarea;
            }
            catch (Exception error)
            {
                await RegistrarErrorAsync(
                    usuarioActual, "consultar una tarea", error);
                throw;
            }
        }

        public async Task<int> CrearAsync(
            Tarea tarea,
            string usuarioActual)
        {
            ValidarUsuarioActual(usuarioActual);

            try
            {
                ValidarTarea(tarea);

                // El repositorio guarda la tarea y su bitácora juntas.
                return await _tareas.CrearAsync(tarea, usuarioActual);
            }
            catch (Exception error)
            {
                await RegistrarErrorAsync(
                    usuarioActual, "crear una tarea", error);
                throw;
            }
        }

        public async Task<bool> ActualizarAsync(
            Tarea tarea,
            string usuarioActual)
        {
            ValidarUsuarioActual(usuarioActual);

            try
            {
                ValidarTarea(tarea);
                ValidarIdentificador(tarea.IdTarea, nameof(tarea.IdTarea));

                return await _tareas.ActualizarAsync(tarea, usuarioActual);
            }
            catch (Exception error)
            {
                await RegistrarErrorAsync(
                    usuarioActual, "actualizar una tarea", error);
                throw;
            }
        }

        public async Task<bool> EliminarAsync(
            int idTarea,
            int idSolicitud,
            string usuarioActual)
        {
            ValidarUsuarioActual(usuarioActual);

            try
            {
                ValidarIdentificador(idTarea, nameof(idTarea));
                ValidarIdentificador(idSolicitud, nameof(idSolicitud));

                return await _tareas.EliminarAsync(
                    idTarea, idSolicitud, usuarioActual);
            }
            catch (Exception error)
            {
                await RegistrarErrorAsync(
                    usuarioActual, "eliminar una tarea", error);
                throw;
            }
        }

        // Aplica las restricciones que exige la tabla Tareas.
        private static void ValidarTarea(Tarea tarea)
        {
            ArgumentNullException.ThrowIfNull(tarea);

            ValidarIdentificador(tarea.IdSolicitud, nameof(tarea.IdSolicitud));
            ValidarIdentificador(tarea.IdUsuario, nameof(tarea.IdUsuario));

            if (string.IsNullOrWhiteSpace(tarea.Descripcion)
                || tarea.Descripcion.Trim().Length < 3)
            {
                throw new ArgumentException(
                    "La descripción debe tener al menos tres caracteres.");
            }

            if (tarea.HorasInvertidas <= 0 || tarea.HorasInvertidas > 24)
            {
                throw new ArgumentException(
                    "Las horas invertidas deben ser mayores que cero y hasta 24.");
            }

            var fecha = tarea.Fecha.Date;

            if (fecha < new DateTime(2000, 1, 1)
                || fecha > new DateTime(2150, 12, 31))
            {
                throw new ArgumentException(
                    "La fecha debe estar entre el 01/01/2000 y el 31/12/2150.");
            }
        }

        private static void ValidarIdentificador(int id, string nombre)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nombre);
            }
        }

        private static void ValidarUsuarioActual(string usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario))
            {
                throw new ArgumentException(
                    "Se requiere identificar al usuario actual.",
                    nameof(usuario));
            }
        }

        // Un fallo de bitácora no debe sustituir el error original.
        private async Task RegistrarErrorAsync(
            string usuario,
            string accion,
            Exception error)
        {
            try
            {
                await _bitacoras.RegistrarErrorAsync(
                    usuario,
                    accion,
                    error.GetType().Name);
            }
            catch
            {
                // El método que llamó conserva y propaga el error original.
            }
        }
    }
}