using PublicaSA.Entities;

namespace PublicaSA.BusinessLogic.Interfaces
{
    public interface ITareaSolicitudService
    {
        // Consulta las tareas paginadas de una solicitud.
        Task<(IEnumerable<TareaSolicitud> Tareas, int Total)>
            ListarPorSolicitudAsync(
                int idSolicitud,
                int pagina,
                int idUsuarioActual);

        // Solicita guardar una tarea y su bitácora.
        Task<int> CrearAsync(
            TareaSolicitud tarea,
            int idUsuarioActual);

        // Consulta una tarea para precargar el formulario de edición.
        Task<TareaSolicitud?> ObtenerPorIdAsync(
            int idTarea,
            int idSolicitud,
            int idUsuarioActual);

        // Actualiza la tarea e identifica al usuario que realiza el cambio.
        Task<bool> ActualizarAsync(
            TareaSolicitud tarea,
            int idUsuarioActual);

        // Elimina una tarea e identifica al usuario que realiza la acción.
        Task<bool> EliminarAsync(
            int idTarea,
            int idSolicitud,
            int idUsuarioActual);

        // Obtiene los usuarios para el formulario de tareas.
        Task<IEnumerable<UsuarioSeleccion>> ListarUsuariosAsync(
            int idUsuarioActual);
    }
}