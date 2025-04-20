

namespace Backend.Services
{
    public class LoggingService : IHostedService
    {
        public async Task StartAsync(CancellationToken cancellationToken)
            => await Core.Services.LoggingService.Instance.StartAsync(cancellationToken);

        public async Task StopAsync(CancellationToken cancellationToken)
            => await Core.Services.LoggingService.Instance.StopAsync(cancellationToken);
    }
}
