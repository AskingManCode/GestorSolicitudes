using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PublicaSA.Services.Interfaces;

namespace PublicaSA.Website.BackgroundServices
{
    public class SolicitudVencimientoBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SolicitudVencimientoBackgroundService> _logger;

        public SolicitudVencimientoBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<SolicitudVencimientoBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Crea un ámbito para utilizar el servicio y sus repositorios.
                    using var scope = _scopeFactory.CreateScope();

                    var servicio = scope.ServiceProvider
                        .GetRequiredService<ISolicitudVencimientoService>();

                    // Usa la fecha de Costa Rica, independientemente del servidor.
                    DateTime fechaActual =
                        DateTimeOffset.UtcNow.ToOffset(
                            TimeSpan.FromHours(-6)).Date;

                    await servicio.ProcesarAsync(fechaActual);
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception error)
                {
                    _logger.LogError(
                        error,
                        "Falló el proceso automático de vencimiento.");
                }

                try
                {
                    // Repite la revisión cada hora mientras la aplicación funciona.
                    await Task.Delay(
                        TimeSpan.FromHours(1),
                        stoppingToken);
                }
                catch (OperationCanceledException)
                    when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }
    }
}