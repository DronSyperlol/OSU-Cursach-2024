using Core.Interfaces;
using Database;
using System.Reflection;

namespace Core.Services
{
    public class CoreService
    {
        static CoreService? instance = null;
        private readonly static object x = new();
        public static CoreService Instance
        {
            get
            {
                lock (x)
                {
                    instance ??= new CoreService();
                    return instance;
                }
            }
        }

        public CoreService()
        {
            if (instance != null)
                throw new InvalidOperationException("Is a singleton. For get instance use a CoreService.Instance property");
        }

        readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(1));

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            IWorker[] workers = Assembly
                .GetExecutingAssembly()
                .GetTypes().Where(t =>
                    t.IsAssignableTo(typeof(IWorker)) &&
                    t.IsInterface == false)
                .Select(t => (IWorker)Activator.CreateInstance(t)!)
                .ToArray()!;
            Task? lastTask = null;
            try
            {
                while (await _timer.WaitForNextTickAsync(stoppingToken))
                {
                    var context = new ApplicationContext();
                    foreach (IWorker worker in workers)
                    {
                        try
                        {
                            lastTask = worker.Handle(context);
                            await lastTask;
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine($"Critical error in CoreService: {ex}");
                        }
                    }
                    lastTask = null;
                }
            }
            catch (OperationCanceledException) { if (lastTask != null) await lastTask; }
        }
    }
}
