using PublicaSA.Entities;
using PublicaSA.Repository.Interfaces;
using PublicaSA.Services.Interfaces;

namespace PublicaSA.Services.Services
{
    public class AutenticacionService : IAutenticacionService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly IBitacoraService _bitacoraService;
        private const int MaxIntentos = 3;

        public AutenticacionService(
            IUsuarioRepository usuarioRepository,
            IEncryptionService encryptionService,
            IBitacoraService bitacoraService)
        {
            _usuarioRepository = usuarioRepository;
            _encryptionService = encryptionService;
            _bitacoraService = bitacoraService;
        }

        public async Task<ResultadoAutenticacion> AutenticarAsync(string nombreUsuario, string contrasena)
        {
            var usuario = await _usuarioRepository.ObtenerPorNombreUsuarioAsync(nombreUsuario);

            // Usuario no existe
            if (usuario == null)
            {
                await _bitacoraService.RegistrarAccionAsync(
                    nombreUsuario,
                    "LOGIN_FALLIDO",
                    new { Motivo = "Usuario no encontrado" });

                return new ResultadoAutenticacion
                {
                    Exitoso = false,
                    Mensaje = "Usuario y/o contraseña incorrectos."
                };
            }

            // Usuario inactivo
            if (usuario.CodigoEstado == "INACTIVO")
            {
                await _bitacoraService.RegistrarAccionAsync(
                    usuario.NombreUsuario,
                    "LOGIN_FALLIDO",
                    new { Motivo = "Usuario inactivo", usuario.IdUsuario });

                return new ResultadoAutenticacion
                {
                    Exitoso = false,
                    Mensaje = "El usuario se encuentra inactivo."
                };
            }

            // Usuario bloqueado
            if (usuario.CodigoEstado == "BLOQUEADO")
            {
                await _bitacoraService.RegistrarAccionAsync(
                    usuario.NombreUsuario,
                    "LOGIN_FALLIDO",
                    new { Motivo = "Usuario bloqueado", usuario.IdUsuario });

                return new ResultadoAutenticacion
                {
                    Exitoso = false,
                    Mensaje = "El usuario se encuentra bloqueado."
                };
            }

            // Comparar contraseña
            string contrasenaDescifrada;
            try
            {
                contrasenaDescifrada = _encryptionService.Decrypt(usuario.Contrasena);
            }
            catch
            {
                await _bitacoraService.RegistrarErrorAsync(
                    usuario.NombreUsuario,
                    "LOGIN",
                    "Error al descifrar la contraseña");

                return new ResultadoAutenticacion
                {
                    Exitoso = false,
                    Mensaje = "Usuario y/o contraseña incorrectos."
                };
            }

            if (contrasenaDescifrada == contrasena)
            {
                // Login exitoso -> reiniciar intentos
                await _usuarioRepository.ActualizarIntentosYEstadoAsync(usuario.IdUsuario, 0);

                await _bitacoraService.RegistrarAccionAsync(
                    usuario.NombreUsuario,
                    "LOGIN_EXITOSO",
                    new { IdUsuario = usuario.IdUsuario, NombreCompleto = usuario.NombreCompleto });

                return new ResultadoAutenticacion
                {
                    Exitoso = true,
                    IdUsuario = usuario.IdUsuario,
                    NombreUsuario = usuario.NombreUsuario,
                    NombreCompleto = usuario.NombreCompleto,
                    CodigoEstado = usuario.CodigoEstado
                };
            }

            // Login fallido -> incrementar intentos
            int nuevosIntentos = usuario.IntentosFallidos + 1;
            string? nuevoEstado = null;

            if (nuevosIntentos >= MaxIntentos)
            {
                nuevoEstado = "BLOQUEADO";
            }

            await _usuarioRepository.ActualizarIntentosYEstadoAsync(usuario.IdUsuario, nuevosIntentos, nuevoEstado);

            await _bitacoraService.RegistrarAccionAsync(
                usuario.NombreUsuario,
                nuevosIntentos >= MaxIntentos ? "USUARIO_BLOQUEADO" : "LOGIN_FALLIDO",
                new
                {
                    IdUsuario = usuario.IdUsuario,
                    Intentos = nuevosIntentos,
                    Motivo = "Contraseña incorrecta"
                });

            return new ResultadoAutenticacion
            {
                Exitoso = false,
                Mensaje = nuevosIntentos >= MaxIntentos
                    ? "El usuario ha sido bloqueado por superar el número de intentos fallidos."
                    : "Usuario y/o contraseña incorrectos."
            };
        }
    }
}