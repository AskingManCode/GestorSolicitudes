using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PublicaSA.Entities;
using PublicaSA.Services.Interfaces;

namespace PublicaSA.Website.Pages.Usuarios
{
    public class CreateModel : PageModel
    {
        private readonly IEstadosUsuarioService _estadosUsuarioService;
        public IEnumerable<EstadoUsuario> ListaEstados { get; set; } = new List<EstadoUsuario>();
        public string EstadoActual { get; set; } = "ACTIVO";

        public CreateModel(IEstadosUsuarioService estadosUsuarioService)
        {
            _estadosUsuarioService = estadosUsuarioService;
        }

        public async Task OnGetAsync()
        {
            ListaEstados = await _estadosUsuarioService.ListarEstadosUsuario();
        }
    }
}