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
        private readonly IValidator<UsuarioEditRequest> _validatorEdit;

        public UsuarioService(
            IUsuarioRepository repository,
            IEncryptionService encryptionService,
            IValidator<UsuarioRequest> validator,
            IValidator<UsuarioEditRequest> validatorEdit)
        {
            _repository = repository;
            _encryptionService = encryptionService;
            _validator = validator;
            _validatorEdit = validatorEdit;
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
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
            
            request.Contrasena = _encryptionService.Encrypt(request.Contrasena);

            return await _repository.Crear(request);
        }

        public async Task<UsuarioResponse> CambiarEstado(int idUsuario, string codigoEstado)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("El ID de usuario no es válido.");

            if (string.IsNullOrWhiteSpace(codigoEstado))
                throw new ArgumentException("El estado es requerido.");

            return await _repository.CambiarEstado(idUsuario, codigoEstado);
        }

        public async Task<UsuarioRequest?> ObtenerPorId(int idUsuario)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("El ID de usuario no es válido.");

            return await _repository.ObtenerPorId(idUsuario);
        }

        public async Task<UsuarioResponse> Editar(UsuarioEditRequest request)
        {
            var validationResult = await _validatorEdit.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            string? contrasenaNuevaCifrada = null;

            if (!string.IsNullOrWhiteSpace(request.ContrasenaNueva))
            {
                var contrasenaCifradaBD = await _repository.ObtenerContrasenaUsuario(request.IdUsuario);

                var contrasenaActualBD = _encryptionService.Decrypt(contrasenaCifradaBD);

                if (contrasenaActualBD != request.ContrasenaActual)
                {
                    throw new ValidationException("La contraseña actual es incorrecta.");
                }

                contrasenaNuevaCifrada = _encryptionService.Encrypt(request.ContrasenaNueva);
            }
            
            var requestSP = new UsuarioEditRequest
            {
                IdUsuario = request.IdUsuario,
                NombreCompleto = request.NombreCompleto,
                Correo = request.Correo,
                NombreUsuario = request.NombreUsuario,
                CodigoEstado = request.CodigoEstado,
                ContrasenaNueva = contrasenaNuevaCifrada
            };

            return await _repository.Editar(requestSP);
        }
        public async Task Eliminar(int idUsuario)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("El ID de usuario no es válido.");

            var eliminado = await _repository.Eliminar(idUsuario);

            if (!eliminado)
                throw new InvalidOperationException("No se pudo eliminar el usuario.");
        }
    }
}