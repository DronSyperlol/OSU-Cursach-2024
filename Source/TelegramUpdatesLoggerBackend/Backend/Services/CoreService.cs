
using Database;
using Core.Interfaces;
using System.Reflection;

namespace Backend.Services
{
    public class CoreService : BackgroundService
    {
        static readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(1));

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            => await Core.Services.CoreService.Instance.ExecuteAsync(stoppingToken);
    }
}
