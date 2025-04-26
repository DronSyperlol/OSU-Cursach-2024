using Config;
using Core.Services.Types;
using Core.Workers;
using Core.Workers.Types;
using Database;
using Microsoft.EntityFrameworkCore;
using TL;

namespace Core.Services
{
    public class LoggingService
    {
        static LoggingService? instance = null;
        private static readonly object x = new();
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

        public async Task Update(CancellationToken cancellationToken)
        {
            if (InWork == false) return;
            using var context = new ApplicationContext();
            var enabledTargetsLogs = await context.Targets
                .AsNoTracking()
                .Include(t => t.FromAccount)
                .GroupBy(t => t.PeerId)
                .Select(g => g.Where(t => t.Id == g.Max(t => t.Id)))
                .ToListAsync(cancellationToken);
            var enabledTargets = enabledTargetsLogs
                .Select(etl => etl.First())
                .Where(etl => etl.Status == Database.Enum.LoggingTargetStatus.Enabled)
                .GroupBy(ge => ge.FromAccount.PhoneNumber)
                .Select(g => new
                {
                    Account = g.FirstOrDefault()!.FromAccount,
                    Targets = g.Select(ge => new
                    {
                        Target = ge,
                        InputPeer = GetInputPeer(ge.PeerId, ge.AccessHash)
                    })
                })
                .ToList();
            foreach (var targets in enabledTargets)
            {
                Loggers.TryGetValue(targets.Account.Id, out List<UpdatesLogger>? loggers);
                LoadedAccount? lacc = null;
                loggers ??= [];
                if (loggers.Count == 0)
                {
                    loggers = [];
                    lacc = await LoadedAccountsWorker.Get(targets.Account.OwnerId, targets.Account.PhoneNumber);
                    lacc.Client.WithUpdateManager((update) => UpdateHandler(update, targets.Account.Id));
                    lacc.OnRestarted += () =>
                    {
                        Console.WriteLine("Account restarted. Set update handler again...");
                        lacc.Client.WithUpdateManager(
                            (update) => UpdateHandler(update, targets.Account.Id));
                        Console.WriteLine("Account restarted. Set update handler again...    done");
                    };
                    Loggers.TryAdd(targets.Account.Id, loggers);
                }
                else
                {
                    lacc = loggers.First().LoadedAccount;
                    loggers.Clear();
                }
                foreach (var target in targets.Targets)
                {
                    var tmp = new UpdatesLogger(lacc, targets.Account, target.Target, target.InputPeer);
                    loggers.Add(tmp);
                }
            }
        }

        public async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(ProgramConfig.LoggingSaveSec));
            Task[]? tasks = null;
            try
            {
                await StartAsync(stoppingToken);
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    foreach (var item in Loggers)
                    {
                        tasks = [.. item.Value.Select(l => l.Save())];
                        await WaitAllAsync(tasks);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                await WaitAllAsync(tasks);
                await StopAsync();
            }
        }

        async Task StartAsync(CancellationToken cancellationToken)
        {
            InWork = true;
            await Update(cancellationToken);
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

        async Task UpdateHandler(Update update, long fromAccountId)
        {
            Loggers.TryGetValue(fromAccountId, out List<UpdatesLogger>? loggers);
            if (loggers == null) return;
            Task? logTask = null;
            switch (update)
            {
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

        public static InputPeer GetInputPeer(long peerId, long? accessHash)
        {
            if (peerId > 0) return new InputPeerUser(peerId, accessHash ?? 0);
            else if (accessHash == null) return new InputPeerChat(peerId);
            else return new InputPeerChannel(peerId, accessHash ?? 0);
        }

        static async Task WaitAllAsync(Task[]? tasks)
        {
            if (tasks == null) return;
            foreach (var task in tasks)
                await task;
        }
    }
}
