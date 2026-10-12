using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PublicaSA.Entities;
using PublicaSA.Services.Interfaces;

namespace PublicaSA.Website.Pages.Usuarios
{
    public class IndexModel : PageModel
    {
        private readonly IEstadosUsuarioService _estadosUsuarioService;
        public IEnumerable<EstadoUsuario> ListaEstados { get; set; } = new List<EstadoUsuario>();

        public IndexModel(IEstadosUsuarioService estadosService)
        {
            _estadosUsuarioService = estadosService;
        }

        public async Task OnGetAsync()
        {
            ListaEstados = await _estadosUsuarioService.ListarEstadosUsuario();
        }
    }
}
