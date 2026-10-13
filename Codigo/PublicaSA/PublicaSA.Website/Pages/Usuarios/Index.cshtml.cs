using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PublicaSA.Entities;
using PublicaSA.Services.Interfaces;

namespace PublicaSA.Website.Pages.Usuarios
{
    public class IndexModel : PageModel
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IEstadosUsuarioService _estadosUsuarioService;

        public IEnumerable<UsuarioResponse> ListaUsuarios { get; set; } = new List<UsuarioResponse>();
        public IEnumerable<EstadoUsuario> ListaEstados { get; set; } = new List<EstadoUsuario>();
        
        public int TotalPaginas { get; set; } = 1;
        public int PaginaActual { get; set; } = 1;

        public IndexModel(IUsuarioService usuarioService, IEstadosUsuarioService estadosUsuarioService)
        {
            _usuarioService = usuarioService;
            _estadosUsuarioService = estadosUsuarioService;
        }

        public async Task OnGetAsync(int pagina = 1)
        {
            // Carga estados para los dropdowns
            ListaEstados = await _estadosUsuarioService.ListarEstadosUsuario();

            // Carga usuarios paginados
            var (usuarios, totalPaginas, paginaActual) = await _usuarioService.ListarUsuarios(pagina, 10);
            ListaUsuarios = usuarios;
            TotalPaginas = totalPaginas;
            PaginaActual = paginaActual;
        }
    }
}