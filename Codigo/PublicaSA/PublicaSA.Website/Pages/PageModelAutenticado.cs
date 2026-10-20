using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PublicaSA.Website.Pages
{
    public class PageModelAutenticado : PageModel
    {
        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            if (!TieneSesion())
            {
                context.Result = RedirigirAlLogin();
                return;
            }

            base.OnPageHandlerExecuting(context);
        }

        private bool TieneSesion()
        {
            var idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            var nombreUsuario = HttpContext.Session.GetString("NombreUsuario");

            return idUsuario.HasValue
                && idUsuario.Value > 0
                && !string.IsNullOrWhiteSpace(nombreUsuario);
        }

        private IActionResult RedirigirAlLogin()
        {
            TempData["Mensaje"] = "Por favor inicie sesión para utilizar el sistema";
            return RedirectToPage("/Login/Index");
        }
    }
}
