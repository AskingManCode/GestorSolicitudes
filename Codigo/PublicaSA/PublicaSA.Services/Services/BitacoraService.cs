using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PublicaSA.Entities;
using PublicaSA.Repository.Interfaces;
using PublicaSA.Services.Interfaces;

namespace PublicaSA.Services.Services
{
    public class BitacoraService : IBitacoraService
    {
        private readonly IBitacoraRepository _bitacoraRepository;
        private readonly ILogger<BitacoraService>? _logger;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
        };

        public BitacoraService(IBitacoraRepository bitacoraRepository, ILogger<BitacoraService>? logger = null)
        {
            _bitacoraRepository = bitacoraRepository;
            _logger = logger;
        }

        public async Task RegistrarAccionAsync(string usuario, string accion, object detalle)
        {
            try
            {
                string jsonDetalle = detalle is string str ? str : JsonSerializer.Serialize(detalle, JsonOptions);
                var bitacora = new Bitacora
                {
                    FechaBitacora = DateTime.Now,
                    UsuarioEjecuta = string.IsNullOrWhiteSpace(usuario) ? "Sistema" : usuario,
                    Accion = accion,
                    Detalle = jsonDetalle
                };

                await _bitacoraRepository.InsertAsync(bitacora);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error al registrar acción en bitácora. Acción: {Accion}, Usuario: {Usuario}", accion, usuario);
            }
        }

        public async Task RegistrarErrorAsync(string usuario, string accion, string mensajeError)
        {
            try
            {
                var bitacora = new Bitacora
                {
                    FechaBitacora = DateTime.Now,
                    UsuarioEjecuta = string.IsNullOrWhiteSpace(usuario) ? "Sistema" : usuario,
                    Accion = $"ERROR: {accion}",
                    Detalle = JsonSerializer.Serialize(new { Error = mensajeError, Fecha = DateTime.Now }, JsonOptions)
                };

                await _bitacoraRepository.InsertAsync(bitacora);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error al registrar error en bitácora. Acción: {Accion}, Usuario: {Usuario}", accion, usuario);
            }
        }
    }
}
