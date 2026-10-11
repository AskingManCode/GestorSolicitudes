using PublicaSA.BusinessLogic.Interfaces;

namespace PublicaSA.WebPresentation.BackgroundServices
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
                    // Crea un ámbito para utilizar los servicios registrados como Scoped.
                    using var scope = _scopeFactory.CreateScope();

                    var servicio = scope.ServiceProvider
                        .GetRequiredService<ISolicitudVencimientoService>();

                    // Costa Rica utiliza UTC-6; obtiene su fecha actual.
                    DateTime fechaActual =
                        DateTime.UtcNow.AddHours(-6).Date;

                    await servicio.ProcesarAsync(fechaActual);
                }
                catch (Exception error)
                {
                    // El servicio registra los errores del procesamiento en la bitácora.
                    _logger.LogError(
                        error,
                        "Falló la ejecución automática de vencimientos.");
                }

                try
                {
                    // Espera una hora y permite detenerse al cerrar la aplicación.
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