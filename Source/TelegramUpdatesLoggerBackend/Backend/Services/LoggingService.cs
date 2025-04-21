

namespace Backend.Services
{
    public class LoggingService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            => await Core.Services.LoggingService.Instance.ExecuteAsync(stoppingToken);
    }
}
