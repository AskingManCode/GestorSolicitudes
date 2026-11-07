using PublicaSA.Entities;

namespace PublicaSA.Services.Interfaces
{
    public interface ITareaService
    {

        // Solicita los usuarios e identifica a quien realiza la consulta.
        Task<IEnumerable<Usuario>> ListarUsuariosAsync(string usuarioActual);

        // Devuelve las tareas y su cantidad para paginar el listado.
        Task<(IEnumerable<Tarea> Tareas, int Total)>
            ListarPorSolicitudAsync(
                int idSolicitud,
                int pagina,
                string usuarioActual);

        Task<Tarea?> ObtenerPorIdAsync(
            int idTarea,
            int idSolicitud,
            string usuarioActual);

        // El servicio valida los datos antes de enviarlos al repositorio.
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