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
                .Where(t => t.Status == Database.Enum.LoggingTargetStatus.Enable)
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
                case UpdateNewMessage unm: logTask = GetLogger(loggers, unm.message.Peer.ID)?.HandleNewMessage(unm); break;
                case UpdateEditMessage uem: logTask = GetLogger(loggers, uem.message.Peer.ID)?.HandleEditMessage(uem); break;
                case UpdateDeleteMessages udm:
                    {
                        // TODO... После получения истории сообщений, сравнивать их с ранее сохранённой историей в БД и искать удалённые сообщения
                        //Task<Messages_MessagesBase>[] tasks;
                        //Func<InputPeer, Task<Messages_MessagesBase>> getFunc;
                        //if (udm.messages.Max() == udm.messages.Min()) // udm.messages.Length == 1
                        //    getFunc = (inputPeer) => loadedAccount.Client.Messages_GetHistory(
                        //        inputPeer, udm.messages.First(), limit: 1);

                        //else
                        //    getFunc = (inputPeer) => loadedAccount.Client.Messages_GetHistory(
                        //                inputPeer, min_id: udm.messages.Min(), max_id: udm.messages.Max());
                        //tasks = [.. loggers.Select(l => getFunc(l.InputPeer))];
                        //Task.WaitAll(tasks);
                         
                    }
                    break;

            }
            if (logTask != null) await logTask;
        }

        static UpdatesLogger? GetLogger(List<UpdatesLogger> loggers, long peerId)
            => loggers.FirstOrDefault(l => l.PeerId == peerId);
    }
}
