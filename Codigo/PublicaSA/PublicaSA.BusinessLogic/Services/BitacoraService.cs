using System;
using System.Text.Json;
using System.Threading.Tasks;
using PublicaSA.DataAccess.Interfaces;
using PublicaSA.Entities;
using PublicaSA.BusinessLogic.Interfaces;

namespace PublicaSA.BusinessLogic.Services
{
    public class BitacoraService : IBitacoraService
    {
        private readonly IBitacoraRepository _bitacoraRepository;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
        };

        public BitacoraService(IBitacoraRepository bitacoraRepository)
        {
            _bitacoraRepository = bitacoraRepository;
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
            catch
            {
                // En bitácora no dejamos que un fallo de log detenga el flujo principal
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
            catch
            {
                // Silencioso
            }
        }
    }
}
