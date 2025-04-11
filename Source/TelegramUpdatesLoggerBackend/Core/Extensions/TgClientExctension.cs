using Config;
using Core.Types;
using Core.Workers;
using TL;

namespace Core.Extensions
{
    public static class TgClientExctension
    {
        public static async Task<AccountInfo?> GetMe(this LoadedAccount account)
        {
            if (account.Status == LoadedAccount.Statuses.Logged)
            {
                return new()
                {
                    PhoneNumber = account.PhoneNumber,
                    PhotoUrl = ProgramConfig.Path.Static + await AccountManager.DownloadAvatar(account.Client, account.Client.User),
                    Title = account.Client.User.first_name + " " + account.Client.User.last_name,
                    Username = account.Client.User.username
                };
            }
            else return null;
        }
        public static async Task<List<DialogInfo>> GetDialogs(this LoadedAccount account, int offsetId, int limit)
        {
            var dialogs = await account.Client.Messages_GetDialogs(offset_id: offsetId, limit: limit);
            List<DialogInfo> result = [];
            foreach (var dialog in dialogs.Dialogs)
            {
                Message message = (Message)dialogs.Messages.First(m => m.Peer.ID == dialog.Peer.ID);
                DialogInfo? tmp = dialogs.UserOrChat(dialog) switch
                {
                    User user => new(user, await AccountManager.DownloadAvatar(account.Client, user), message.message),
                    Chat chat => new(chat, await AccountManager.DownloadAvatar(account.Client, chat), message.message),
                    Channel channel => new(channel, await AccountManager.DownloadAvatar(account.Client, channel), message.message),
                    _ => null,
                };
                if (tmp != null) result.Add(tmp);
            }
            return result;
        }

        public static async Task<List<object>> GetDialogHistory(this LoadedAccount account, long peerId, InputPeer peer)
        {
            var history = account.Client.Messages_GetHistory(peer);
            throw new NotImplementedException();
        }
    }
}
