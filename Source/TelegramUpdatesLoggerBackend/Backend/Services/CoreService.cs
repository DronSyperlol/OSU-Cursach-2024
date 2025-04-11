
using Database;
using Core.Interfaces;
using System.Reflection;

namespace Backend.Services
{
    public class CoreService : BackgroundService
    {
        static readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(1));

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            IWorker[] workers = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t =>
                    t.IsAssignableTo(typeof(IWorker)) &&
                    t.IsInterface == false)
                .Select(t => (IWorker)Activator.CreateInstance(t)!)
                .ToArray()!;
            try
            {
                while (await _timer.WaitForNextTickAsync(stoppingToken))
                {
                    var context = new ApplicationContext();
                    foreach (IWorker worker in workers)
                        await worker.Handle(context);
                    await context.SaveChangesAsync(stoppingToken);
                }
            }
            catch (OperationCanceledException) { }
        }
    }
}
