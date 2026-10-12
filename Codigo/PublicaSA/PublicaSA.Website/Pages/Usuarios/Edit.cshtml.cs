using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PublicaSA.Entities;
using PublicaSA.Services.Interfaces;

namespace PublicaSA.Website.Pages.Usuarios
{
    public class EditModel : PageModel
    {
        private readonly IEstadosUsuarioService _estadosUsuarioService;
        public IEnumerable<EstadoUsuario> ListaEstados { get; set; } = new List<EstadoUsuario>();
        public string EstadoActual { get; set; } = "ACTIVO";

        public EditModel(IEstadosUsuarioService estadosUsuarioService)
        {
            _estadosUsuarioService = estadosUsuarioService;
        }

        public async Task OnGetAsync(int id)
        {
            ListaEstados = await _estadosUsuarioService.ListarEstadosUsuario();
        }
    }
}