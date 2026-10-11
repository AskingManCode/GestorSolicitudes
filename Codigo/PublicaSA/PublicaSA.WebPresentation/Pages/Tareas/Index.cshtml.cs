using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using PublicaSA.BusinessLogic.Interfaces;
using PublicaSA.Entities;

namespace PublicaSA.WebPresentation.Pages.Tareas
{
    public class IndexModel : PageModel
    {
        private readonly ITareaSolicitudService _tareas;

        public IndexModel(ITareaSolicitudService tareas)
        {
            _tareas = tareas;
        }

        public IEnumerable<TareaSolicitud> Tareas { get; private set; }
            = Array.Empty<TareaSolicitud>();

        public int IdSolicitud { get; private set; }
        public int Pagina { get; private set; }
        public int TotalPaginas { get; private set; }

        public string? Error { get; private set; }

        public async Task<IActionResult> OnGetAsync(
            int idSolicitud,
            int pagina = 1)
        {
            int? idUsuarioActual =
                HttpContext.Session.GetInt32("IdUsuario");

            if (!idUsuarioActual.HasValue || idUsuarioActual.Value <= 0)
            {
                return RedirigirAlLogin();
            }

            if (idSolicitud <= 0 || pagina < 1)
            {
                return BadRequest();
            }

            IdSolicitud = idSolicitud;
            Pagina = pagina;

            try
            {
                // Obtiene diez tareas por página y registra la consulta.
                var resultado = await _tareas.ListarPorSolicitudAsync(
                    IdSolicitud,
                    Pagina,
                    idUsuarioActual.Value);

                Tareas = resultado.Tareas;

                TotalPaginas = Math.Max(
                    1,
                    (int)Math.Ceiling(resultado.Total / 10.0));
            }
            catch (Exception)
            {
                Error = "No se pudieron consultar las tareas.";
            }

            return Page();
        }

        // Recibe la eliminación mediante POST, después de la confirmación.
        public async Task<IActionResult> OnPostEliminarAsync(
            int idTarea,
            int idSolicitud)
        {
            int? idUsuarioActual =
                HttpContext.Session.GetInt32("IdUsuario");

            if (!idUsuarioActual.HasValue || idUsuarioActual.Value <= 0)
            {
                return RedirigirAlLogin();
            }

            if (idTarea <= 0 || idSolicitud <= 0)
            {
                return BadRequest();
            }

            try
            {
                bool eliminada = await _tareas.EliminarAsync(
                    idTarea,
                    idSolicitud,
                    idUsuarioActual.Value);

                if (!eliminada)
                {
                    return NotFound();
                }

                TempData["Exito"] = "La tarea se eliminó correctamente.";
            }
            catch (MySqlException error) when (error.Number == 1451)
            {
                // MySQL impide eliminar registros referenciados por otros datos.
                TempData["Error"] =
                    "No se puede eliminar un registro con datos relacionados.";
            }
            catch (Exception)
            {
                // El servicio registra el error técnico en la bitácora.
                TempData["Error"] = "No se pudo eliminar la tarea.";
            }

            return RedirectToPage(
                "./Index",
                new { idSolicitud });
        }

        private IActionResult RedirigirAlLogin()
        {
            TempData["Mensaje"] =
                "Por favor inicie sesión para utilizar el sistema";

            return RedirectToPage("/Login/Login");
        }
    }
}