using PublicaSA.Entities;
using PublicaSA.Repository.Interfaces;
using PublicaSA.Repository.Repositories;
using PublicaSA.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PublicaSA.Services.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;

        public UsuarioService(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        public async Task<(IEnumerable<UsuarioResponse> ListaUsuarios, int TotalPaginas, int PaginaActual)> ListarUsuarios(int pagina, int tamanoPagina)
        {
            var (listaUsuarios, totalRegistros) = await _repository.ListarUsuarios(pagina, tamanoPagina);
            var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)tamanoPagina);

            return (listaUsuarios, totalPaginas, pagina);
        }
    }
}
