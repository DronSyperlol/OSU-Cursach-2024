using Core.Types;
using Database.Entities;
using TL;

namespace Core.Services.Types
{
    class UpdatesLogger : IDisposable
    {
        readonly List<AccountLog> Logs = [];
        public UpdatesLogger(LoadedAccount lacc, Account fromAccount, long targetDbId, InputPeer peer)
        {
            _lacc = lacc;
            _lacc.InUse = true;
            _account = fromAccount;
            _targetDbId = targetDbId;
            _peer = peer;
        }
        readonly long _targetDbId;
        readonly Account _account;
        readonly LoadedAccount _lacc;
        readonly InputPeer _peer;

        public long AccountId { get => _account.Id; }
        public long PeerId { get => _peer.ID; }

        public async Task Save()
        {
            // TODO сохранение логов

            throw new NotImplementedException();
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
                case Message msg: Console.WriteLine($"New message {msg.id} from {msg.from_id} to {msg.peer_id}: {msg.message}"); break;
                default: Console.WriteLine("Unknown type of the message"); break;
            }
            return Task.CompletedTask;
        }
        public Task HandleEditMessage(UpdateEditMessage update)
        {
            switch (update.message)
            {
                case Message msg: Console.WriteLine($"Edit message {msg.id} from {msg.from_id} to {msg.peer_id}: {msg.message}"); break;
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
