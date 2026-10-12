using PublicaSA.Entities;
using PublicaSA.Repository.Interfaces;
using PublicaSA.Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace PublicaSA.Services.Services
{
    public class UsuarioService : IUsuarioRepository
    {
        private readonly IUsuarioRepository _repository;

        public UsuarioService(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        public async Task<(IEnumerable<UsuarioResponse> ListaUsuarios, int TotalRegistros)> ListarUsuarios(int pagina, int tamanoPagina)
        {
            var (listaUsuarios, totalRegistros) = await _usuarioRepository.ListarUsuarios(pagina, tamanoPagina);
            var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)tamanoPagina);

            return (listaUsuarios, totalPaginas, pagina);
        }
    }
}
