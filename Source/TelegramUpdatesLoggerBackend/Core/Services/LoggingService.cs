using Core.Extensions;
using Core.Services.Types;
using Core.Workers;
using Database;
using Microsoft.EntityFrameworkCore;
using TL;

namespace Core.Services
{
    public class LoggingService
    {
        static LoggingService? instance = null;
        static object x = new object();
        public static LoggingService Instance
        {
            get
            {
                lock (x)
                {
                    instance ??= new LoggingService();
                    return instance;
                }
            }
        }

        public LoggingService()
        {
            if (instance != null)
                throw new InvalidOperationException("Is a singleton. For get instance use a LoggingService.Instance property");
        }

        public bool InWork { get; private set; } = false;

        readonly Dictionary<long, List<UpdatesLogger>> Loggers = [];

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            InWork = true;
            using var context = new ApplicationContext();
            var enabledTargets = context.Targets
                .AsNoTracking()
                .Include(t => t.FromAccount.PhoneNumber)
                .Where(t => t.Status == Database.Enum.LoggingTargetStatus.Enable)
                .GroupBy(t => t.FromAccount)
                .Select(g => new
                {
                    Account = g.Key,
                    Targets = g.Select(ge => new WatchingTarget(ge.Id, TgClientExctension.GetInputPeer(ge.PeerId, ge.AccessHash)))
                });
            foreach (var targets in enabledTargets)
            {
                var lacc = await AccountManager.Get(targets.Account.OwnerId, targets.Account.PhoneNumber);
                List<UpdatesLogger> loggers = [];
                foreach (var target in targets.Targets)
                {
                    var tmp = new UpdatesLogger(lacc, targets.Account, target.TargetDbId, target.InputPeer);
                    loggers.Add(tmp);
                    var manager = lacc.Client.WithUpdateManager(
                        (update) => UpdateHandler(update, tmp));
                }
                Loggers.TryAdd(targets.Account.Id, loggers);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            InWork = false;
            return Task.CompletedTask;
        }

        static async Task UpdateHandler(Update update, UpdatesLogger logger)
        {
            switch (update)
            {
                case UpdateNewMessage unm: await logger.HandleNewMessage(unm); break;
                case UpdateEditMessage uem: await logger.HandleEditMessage(uem); break;
                case UpdateDeleteMessages udm: await logger.HandleDeleteMessages(udm); break;
            }
        }
    }
}
