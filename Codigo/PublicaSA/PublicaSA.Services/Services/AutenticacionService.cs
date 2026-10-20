using PublicaSA.Entities;
using PublicaSA.Repository.Interfaces;
using PublicaSA.Services.Interfaces;

namespace PublicaSA.Services.Services
{
    public class AutenticacionService : IAutenticacionService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEncryptionService _encryptionService;
        private const int MaxIntentos = 3;

        public AutenticacionService(
            IUsuarioRepository usuarioRepository,
            IEncryptionService encryptionService)
        {
            _usuarioRepository = usuarioRepository;
            _encryptionService = encryptionService;
        }

        public async Task<ResultadoAutenticacion> AutenticarAsync(string nombreUsuario, string contrasena)
        {
            var usuario = await _usuarioRepository.ObtenerPorNombreUsuarioAsync(nombreUsuario);

            // Usuario no existe
            if (usuario == null)
            {
                return new ResultadoAutenticacion
                {
                    Exitoso = false,
                    Mensaje = "Usuario y/o contraseña incorrectos."
                };
            }

            // Usuario inactivo
            if (usuario.CodigoEstado == "INACTIVO")
            {
                return new ResultadoAutenticacion
                {
                    Exitoso = false,
                    Mensaje = "El usuario se encuentra inactivo."
                };
            }

            // Usuario bloqueado
            if (usuario.CodigoEstado == "BLOQUEADO")
            {
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
                return new ResultadoAutenticacion
                {
                    Exitoso = false,
                    Mensaje = "Usuario y/o contraseña incorrectos."
                };
            }

            if (contrasenaDescifrada == contrasena)
            {
                // Login exitoso → reiniciar intentos
                await _usuarioRepository.ActualizarIntentosYEstadoAsync(usuario.IdUsuario, 0);

                return new ResultadoAutenticacion
                {
                    Exitoso = true,
                    IdUsuario = usuario.IdUsuario,
                    NombreUsuario = usuario.NombreUsuario,
                    NombreCompleto = usuario.NombreCompleto,
                    CodigoEstado = usuario.CodigoEstado
                };
            }

            // Login fallido → incrementar intentos
            int nuevosIntentos = usuario.IntentosFallidos + 1;
            string? nuevoEstado = null;

            if (nuevosIntentos >= MaxIntentos)
            {
                nuevoEstado = "BLOQUEADO";
            }

            await _usuarioRepository.ActualizarIntentosYEstadoAsync(usuario.IdUsuario, nuevosIntentos, nuevoEstado);

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