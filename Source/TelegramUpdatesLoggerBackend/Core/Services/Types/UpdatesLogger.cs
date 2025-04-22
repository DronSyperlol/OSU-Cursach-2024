using Core.Types;
using Database;
using Database.Entities;
using Microsoft.EntityFrameworkCore;
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
        public LoadedAccount LoadedAccount { get => _lacc; }

        public async Task Save()
        {
            UpdateLog[] logsToSave;
            lock (UpdatesLogs)
            {
                logsToSave = [.. UpdatesLogs];
                UpdatesLogs.Clear();
            }
            if (logsToSave.Length == 0) return;
            using var context = new ApplicationContext();
            try
            {
                context.Targets.Attach(_target);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
            try
            {
                context.Updates.AttachRange([.. logsToSave.OfType<UpdateMessageLog>().Where(l => l.PrevEdit != null).Select(l => l.PrevEdit!)]);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
            await context.Updates.AddRangeAsync([.. logsToSave]);
            await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Save().Wait();
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
                        MsgDate = msg.date
                    });
                    break;
                default: Console.WriteLine("Unknown type of the message"); break;
            }
            return Task.CompletedTask;
        }
        public async Task HandleEditMessage(UpdateEditMessage update)
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
                        PrevEdit = await GetPrevEdit(msg.id),
                        Time = DateTime.UtcNow,
                        MsgDate = msg.edit_date
                    });
                    break;
                default: Console.WriteLine("Unknown type of the message"); break;
            }
        }
        public Task HandleDeleteMessages(UpdateDeleteMessages update)
        {
            var sharedTime = DateTime.UtcNow;
            foreach (var msgId in update.messages)
            {
                UpdatesLogs.Add(new UpdateDeleteMessageLog()
                {
                    LoggingTarget = _target,
                    MessageId = msgId,
                    Time = sharedTime,
                });
            }
            return Task.CompletedTask;
        }

        async Task<UpdateMessageLog?> GetPrevEdit(int messageId)
        {
            var prevEdit = UpdatesLogs.OfType<UpdateMessageLog>()
                .OrderBy(l => l.Time).FirstOrDefault(l => l.MessageId == messageId);
            if (prevEdit == null)
            {
                using var context = new ApplicationContext();
                prevEdit = await context.UpdatesMessage
                    .AsNoTracking()
                    .Where(l =>
                        l.LoggingTargetId == _target.Id &&
                        l.LoggingTarget.PeerId == PeerId)
                    .OrderByDescending(l => l.Time)
                    .FirstOrDefaultAsync(l => l.MessageId == messageId);
            }
            return prevEdit;
        }
    }
}
