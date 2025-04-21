using Core.Types;
using Database;
using Database.Entities;
using System.Text.Json;
using TL;

namespace Core.Services.Types
{
    class UpdatesLogger : IDisposable
    {
        readonly List<UpdateLog> UpdatesLogs = [];
        public UpdatesLogger(LoadedAccount lacc, Account fromAccount, LoggingTarget target, InputPeer peer)
        {
            _lacc = lacc;
            _lacc.InUse = true;
            _account = fromAccount;
            _target = target;
            _peer = peer;
        }
        readonly LoggingTarget _target;
        readonly Account _account;
        readonly LoadedAccount _lacc;
        readonly InputPeer _peer;

        public long AccountId { get => _account.Id; }
        public long PeerId { get => _peer.ID; }
        public InputPeer InputPeer { get => _peer; }

        public async Task Save()
        {
            UpdateLog[] LogsToSave;
            lock (UpdatesLogs)
            {
                LogsToSave = [.. UpdatesLogs];
                UpdatesLogs.Clear();
            }
            if (LogsToSave.Length == 0) return;
            using var context = new ApplicationContext();
            context.Targets.Attach(_target);
            await context.Updates.AddRangeAsync([.. LogsToSave]);
            await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Save();
            _lacc.InUse = false;
        }

        public Task HandleNewMessage(UpdateNewMessage update)
        {
            switch (update.message)
            {
                case Message msg:
                    UpdatesLogs.Add(new UpdateMessageLog()
                    {
                        LoggingTarget = _target,
                        Text = msg.message,
                        TextEntities = JsonSerializer.Serialize(msg.entities),
                        MessageId = msg.id,
                        PrevEdit = null,
                        Time = DateTime.UtcNow,
                        MsgDate = msg.Date
                    });
                    break;
                default: Console.WriteLine("Unknown type of the message"); break;
            }
            return Task.CompletedTask;
        }
        public Task HandleEditMessage(UpdateEditMessage update)
        {
            switch (update.message)
            {
                case Message msg:
                    UpdatesLogs.Add(new UpdateMessageLog()
                    {
                        LoggingTarget = _target,
                        Text = msg.message,
                        TextEntities = JsonSerializer.Serialize(msg.entities),
                        MessageId = msg.id,
                        PrevEdit = null,
                        Time = DateTime.UtcNow,
                        MsgDate = msg.Date
                    });
                    break;
                default: Console.WriteLine("Unknown type of the message"); break;
            }
            return Task.CompletedTask;
        }
        public Task HandleDeleteMessages(UpdateDeleteMessages update)
        {
            Console.WriteLine($"Deleted messages from {update.messages} (нужно изучить)");
            return Task.CompletedTask;
        }
    }
}
