using Core.Interfaces;
using Database;
using System.Reflection;

namespace Core
{
    public class CoreMain   //  Класс, отвечающий за таймеры
    {
        static readonly PeriodicTimer _timer;
        static Task? _timerTask;
        static readonly CancellationTokenSource _cancellationTokenSource = new();
        static IWorker[] Workers { get; }
        static CoreMain()
        {
            // Получаем все типы наследованные от IWorker
            var workers = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t =>
                    t.IsAssignableTo(typeof(IWorker)) &&
                    t.IsInterface == false)
                .ToArray();
            var instances = workers.Select(t => Activator.CreateInstance(t)).ToArray();
            Workers = instances.Where(i => i != null).Select(i => (IWorker)i!).ToArray()!;

            _timer = new(TimeSpan.FromSeconds(1));
        }
        public static void Start()
        {
            _timerTask = Repeater();
        }
        static async Task Repeater()
        {
            try
            {
                while (await _timer.WaitForNextTickAsync(_cancellationTokenSource.Token))
                {
                    var context = new ApplicationContext();
                    foreach (IWorker worker in Workers)
                        await worker.Handle(context);
                    await context.SaveChangesAsync();
                }
            }
            catch (OperationCanceledException) { }
        }
        public static async Task StopAsync()
        {
            if (_timerTask == null) { return; }
            _cancellationTokenSource.Cancel();
            await _timerTask;
            _timerTask = null;
            _cancellationTokenSource.Dispose();
        }

        //static void CreateDirs()
        //{
        //    Directory.CreateDirectory(ProgramConfig.TelegramApiAuth.SessionsDir);
        //}
    }
}
