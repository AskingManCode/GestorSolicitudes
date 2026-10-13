using FluentValidation;
using PublicaSA.Entities;
using PublicaSA.Repository.Interfaces;
using PublicaSA.Services.Interfaces;

namespace PublicaSA.Services.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repository;
        private readonly IEncryptionService _encryptionService;
        private readonly IValidator<UsuarioRequest> _validator;

        public UsuarioService(
            IUsuarioRepository repository,
            IEncryptionService encryptionService,
            IValidator<UsuarioRequest> validator)
        {
            _repository = repository;
            _encryptionService = encryptionService;
            _validator = validator;
        }

        public async Task<(IEnumerable<UsuarioResponse> ListaUsuarios, int TotalPaginas, int PaginaActual)> ListarUsuarios(int pagina, int tamanoPagina)
        {
            pagina = Math.Max(pagina, 1);
            tamanoPagina = Math.Max(tamanoPagina, 1);

            var (listaUsuarios, totalRegistros) = await _repository.ListarUsuarios(pagina, tamanoPagina);
            var totalPaginas = (int)Math.Ceiling(totalRegistros / (double)tamanoPagina);

            return (listaUsuarios, totalPaginas, pagina);
        }

        public async Task<UsuarioResponse> Crear(UsuarioRequest request)
        {
            // Validacion 
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            // Cifrado
            request.Contrasena = _encryptionService.Encrypt(request.Contrasena);

            return await _repository.Crear(request);
        }

        public async Task<UsuarioResponse> CambiarEstado(int idUsuario, string codigoEstado)
        {
            // Validación simple
            if (idUsuario <= 0)
                throw new ArgumentException("El ID de usuario no es válido.");

            if (string.IsNullOrWhiteSpace(codigoEstado))
                throw new ArgumentException("El estado es requerido.");

            return await _repository.CambiarEstado(idUsuario, codigoEstado);
        }
    }
}