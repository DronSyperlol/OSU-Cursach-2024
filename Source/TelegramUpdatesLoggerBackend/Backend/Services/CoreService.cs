namespace Backend.Services
{
    public class CoreService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            => await Core.Services.CoreService.Instance.ExecuteAsync(stoppingToken);
    }
}
