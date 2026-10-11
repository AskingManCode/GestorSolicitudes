using PublicaSA.Entities;

namespace PublicaSA.DataAccess.Interfaces
{
    public interface ITareaSolicitudRepository
    {
        // Consulta el listado paginado y su cantidad total.
        Task<IEnumerable<TareaSolicitud>> ListarPorSolicitudAsync(
            int idSolicitud,
            int pagina);

        Task<int> ContarPorSolicitudAsync(int idSolicitud);

        // Guarda una tarea y registra su creación.
        Task<int> CrearAsync(
            TareaSolicitud tarea,
            int idUsuarioActual);

        // Obtiene los datos para el formulario de edición.
        Task<TareaSolicitud?> ObtenerPorIdAsync(
            int idTarea,
            int idSolicitud);

        // Actualiza la tarea y registra los datos anteriores y nuevos.
        Task<bool> ActualizarAsync(
            TareaSolicitud tarea,
            int idUsuarioActual);

        // Elimina la tarea de la solicitud indicada y registra quién lo hizo.
        Task<bool> EliminarAsync(
            int idTarea,
            int idSolicitud,
            int idUsuarioActual);

        // Obtiene los usuarios disponibles para el campo de selección.
        Task<IEnumerable<UsuarioSeleccion>> ListarUsuariosAsync();
    }
}