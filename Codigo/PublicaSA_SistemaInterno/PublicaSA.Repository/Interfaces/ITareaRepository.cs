using PublicaSA.Entities;

namespace PublicaSA.Repository.Interfaces
{
    public interface ITareaRepository
    {

        // Obtiene los usuarios disponibles para el formulario de tareas.
        Task<IEnumerable<Usuario>> ListarUsuariosAsync();

        // Consulta las tareas de una solicitud con paginación.
        Task<IEnumerable<Tarea>> ListarPorSolicitudAsync(
            int idSolicitud,
            int pagina);

        Task<int> ContarPorSolicitudAsync(int idSolicitud);

        Task<Tarea?> ObtenerPorIdAsync(
            int idTarea,
            int idSolicitud);

        // Cada cambio recibe el nombre del usuario para la bitácora.
        Task<int> CrearAsync(
            Tarea tarea,
            string usuarioActual);

        Task<bool> ActualizarAsync(
            Tarea tarea,
            string usuarioActual);

        Task<bool> EliminarAsync(
            int idTarea,
            int idSolicitud,
            string usuarioActual);
    }
}