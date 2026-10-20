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
        private readonly IBitacoraService _bitacoraService;

        public UsuarioService(
            IUsuarioRepository repository,
            IEncryptionService encryptionService,
            IValidator<UsuarioRequest> validator,
            IValidator<UsuarioEditRequest> validatorEdit,
            IBitacoraService bitacoraService)
        {
            _repository = repository;
            _encryptionService = encryptionService;
            _validator = validator;
            _validatorEdit = validatorEdit;
            _bitacoraService = bitacoraService;
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

            var resultado = await _repository.Crear(request);

            await _bitacoraService.RegistrarAccionAsync(
                request.NombreUsuario,
                "CREACION_USUARIO",
                new
                {
                    IdUsuario = resultado.IdUsuario,
                    NombreUsuario = resultado.NombreUsuario,
                    NombreCompleto = resultado.NombreCompleto,
                    Correo = resultado.Correo,
                    CodigoEstado = request.CodigoEstado
                });

            return resultado;
        }

        public async Task<UsuarioResponse> CambiarEstado(int idUsuario, string codigoEstado)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("El ID de usuario no es válido.");

            if (string.IsNullOrWhiteSpace(codigoEstado))
                throw new ArgumentException("El estado es requerido.");

            var usuarioAnterior = await _repository.ObtenerPorId(idUsuario);

            var resultado = await _repository.CambiarEstado(idUsuario, codigoEstado);

            await _bitacoraService.RegistrarAccionAsync(
                resultado.NombreUsuario,
                "MODIFICACION_USUARIO",
                new
                {
                    IdUsuario = idUsuario,
                    Accion = "Cambio de estado",
                    Anterior = new { CodigoEstado = usuarioAnterior?.CodigoEstado },
                    Actual = new { CodigoEstado = codigoEstado }
                });

            return resultado;
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

            var usuarioAnterior = await _repository.ObtenerPorId(request.IdUsuario);

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

            var resultado = await _repository.Editar(requestSP);

            await _bitacoraService.RegistrarAccionAsync(
                resultado.NombreUsuario,
                "MODIFICACION_USUARIO",
                new
                {
                    IdUsuario = request.IdUsuario,
                    Accion = "Edición de usuario",
                    CambioContrasena = !string.IsNullOrWhiteSpace(contrasenaNuevaCifrada),
                    Anterior = new
                    {
                        NombreCompleto = usuarioAnterior?.NombreCompleto,
                        Correo = usuarioAnterior?.Correo,
                        NombreUsuario = usuarioAnterior?.NombreUsuario,
                        CodigoEstado = usuarioAnterior?.CodigoEstado
                    },
                    Actual = new
                    {
                        NombreCompleto = request.NombreCompleto,
                        Correo = request.Correo,
                        NombreUsuario = request.NombreUsuario,
                        CodigoEstado = request.CodigoEstado
                    }
                });

            return resultado;
        }

        public async Task Eliminar(int idUsuario)
        {
            if (idUsuario <= 0)
                throw new ArgumentException("El ID de usuario no es válido.");

            var usuario = await _repository.ObtenerPorId(idUsuario);

            var eliminado = await _repository.Eliminar(idUsuario);

            if (!eliminado)
                throw new InvalidOperationException("No se pudo eliminar el usuario.");

            await _bitacoraService.RegistrarAccionAsync(
                usuario?.NombreUsuario ?? "Sistema",
                "ELIMINACION_USUARIO",
                new
                {
                    IdUsuario = idUsuario,
                    NombreUsuario = usuario?.NombreUsuario,
                    NombreCompleto = usuario?.NombreCompleto,
                    Correo = usuario?.Correo
                });
        }
    }
}