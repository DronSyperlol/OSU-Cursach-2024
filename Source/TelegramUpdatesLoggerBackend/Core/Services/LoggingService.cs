using Config;
using Core.Extensions;
using Core.Services.Types;
using Core.Types;
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

        public async Task Update()
        {
            if (InWork == false) return;
            using var context = new ApplicationContext();
            var enabledTargets = await context.Targets
                .AsNoTracking()
                .Include(t => t.FromAccount)
                .Where(t => t.Status == Database.Enum.LoggingTargetStatus.Enabled)
                .GroupBy(t => t.FromAccount)
                .Select(g => new
                {
                    Account = g.Key,
                    Targets = g.Select(ge => new
                    {
                        Target = ge,
                        InputPeer = TgClientExctension.GetInputPeer(ge.PeerId, ge.AccessHash)
                    })
                }).ToListAsync();
            foreach (var targets in enabledTargets)
            {
                var lacc = await AccountManager.Get(targets.Account.OwnerId, targets.Account.PhoneNumber);
                List<UpdatesLogger> loggers = [];
                lacc.Client.WithUpdateManager((update) => UpdateHandler(update, lacc, targets.Account.Id));
                foreach (var target in targets.Targets)
                {
                    var tmp = new UpdatesLogger(lacc, targets.Account, target.Target, target.InputPeer);
                    loggers.Add(tmp);
                }
                Loggers.TryAdd(targets.Account.Id, loggers);
            }
        }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(ProgramConfig.LoggingSaveSec));
            try
            {
                await StartAsync(stoppingToken);
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    foreach (var item in Loggers)
                    {
                        Task[] tasks = [.. item.Value.Select(l => l.Save())];
                        foreach (var task in tasks)
                            await task;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                await StopAsync();
            }
        }

        async Task StartAsync(CancellationToken cancellationToken)
        {
            InWork = true;
            await Update();
        }

        Task StopAsync()
        {
            InWork = false;
            foreach (var item in Loggers)
            {
                item.Value.ForEach(l => l.Dispose());
            }
            return Task.CompletedTask;
        }

        async Task UpdateHandler(Update update, LoadedAccount loadedAccount, long fromAccountId)
        {
            Loggers.TryGetValue(fromAccountId, out List<UpdatesLogger>? loggers);
            if (loggers == null) return;
            Task? logTask = null;
            switch (update)
            {
                case UpdateNewChannelMessage uncm:
                    {
                        Console.WriteLine(uncm.GetType().Name);
                    }
                    break;
                case UpdateDeleteChannelMessages udcm:
                    {
                        Console.WriteLine(udcm.GetType().Name);
                    }
                    break;
                case UpdateNewMessage unm: logTask = GetLogger(loggers, unm.message.Peer.ID)?.HandleNewMessage(unm); break;
                case UpdateEditMessage uem: logTask = GetLogger(loggers, uem.message.Peer.ID)?.HandleEditMessage(uem); break;
                case UpdateDeleteMessages udm:
                    {
                        using var context = new ApplicationContext();
                        var peerId = (await context.UpdatesMessage
                            .AsNoTracking()
                            .Include(u => u.LoggingTarget)
                            .FirstOrDefaultAsync(u => u.MessageId == udm.messages.First()))
                            ?.LoggingTarget.PeerId ?? -1;
                        if (peerId != -1)
                            logTask = GetLogger(loggers, peerId)?.HandleDeleteMessages(udm);
                    }
                    break;
            }
            if (logTask != null) await logTask;
        }

        static UpdatesLogger? GetLogger(List<UpdatesLogger> loggers, long peerId)
            => loggers.FirstOrDefault(l => l.PeerId == peerId);
    }
}
