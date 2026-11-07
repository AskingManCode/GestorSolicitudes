using PublicaSA.Entities;
using PublicaSA.Repository.Interfaces;
using PublicaSA.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PublicaSA.Services.Services
{
    public class EstadosUsuarioService : IEstadosUsuarioService
    {
        private readonly IEstadosUsuarioRepository _repository;

        public EstadosUsuarioService(IEstadosUsuarioRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<EstadoUsuario>> ListarEstadosUsuario()
        {
            return await _repository.ListarEstadosUsuario();
        }
    }
}
