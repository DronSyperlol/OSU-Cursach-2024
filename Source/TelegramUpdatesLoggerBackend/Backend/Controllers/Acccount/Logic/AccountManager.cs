using Backend.Controllers.Acccount.Logic.Types;
using Database;
using Core.Workers;
using Config;
using Core.Workers.Types;
using Database.Enum;
using TL;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers.Acccount.Logic
{
    public static class AccountManager
    {
        public static async Task<string> NewAccount(ApplicationContext context, string phone, long ownerId)
            => await LoadedAccountsWorker.OpenNewAccount(context, phone, ownerId);

        public static async Task<string> NewAccountSetCode(ApplicationContext context, string phone, string code)
            => await LoadedAccountsWorker.SetCodeToNewAccount(context, phone, code);

        public static async Task<string> NewAccountSetCloudPassword(ApplicationContext context, string phone, string password)
            => await LoadedAccountsWorker.SetCloudPasswordToNewAccount(context, phone, password);

        public static async Task<List<AccountInfo>> GetAccounts(ApplicationContext context, long userId)
        {
            List<AccountInfo> result = [];
            var accounts = context.Accounts.Where(a => a.OwnerId == userId);
            foreach (var acc in accounts)
            {
                var account = await LoadedAccountsWorker.Get(userId, acc.PhoneNumber);
                if (account.Status == LoadedAccount.Statuses.Logged)
                {
                    result.Add(new()
                    {
                        PhoneNumber = account.PhoneNumber,
                        PhotoUrl = ProgramConfig.Path.Static + await LoadedAccountsWorker.DownloadAvatar(account.Client, account.Client.User),
                        Title = account.Client.User.first_name + " " + account.Client.User.last_name,
                        Username = account.Client.User.username
                    });
                }
            }
            return result;
        }

        public static async Task<List<DialogInfo>> GetDialogs(
            ApplicationContext context, long userId, string phone,
            int offsetId, int limit, CancellationToken cancellationToken)
        {
            var account = await LoadedAccountsWorker.Get(userId, phone);
            var targets = await context.Targets
                .Include(t => t.FromAccount)
                .Where(t => t.FromAccount.PhoneNumber == account.PhoneNumber)
                .GroupBy(t => t.PeerId)
                .Select(g => new
                {
                    PeerId = g.Key,
                    LastTargetLog = g.Where(t => t.Id == g.Max(t => t.Id)),
                })
                .ToListAsync(cancellationToken);
            InputPeer? offsetPeer = null;
            DateTime offsetDate = default;
            List<DialogInfo> result = [];
            const int maxLimit = 100;
            int prevGet = maxLimit;
            while (result.Count < limit && prevGet >= maxLimit)
            {
                var dialogs = await account.Client.Messages_GetDialogs(limit: maxLimit, offset_id: offsetId, offset_peer: offsetPeer, offset_date: offsetDate);
                prevGet = dialogs.Dialogs.Length;
                foreach (var dialog in dialogs.Dialogs)
                {
                    string topMessage = "<empty>";
                    var msgOfDialog = dialogs.Messages.FirstOrDefault(m => m.Peer.ID == dialog.Peer.ID);
                    switch (msgOfDialog)
                    {
                        case Message msg:
                            topMessage = msg.message;
                            break;
                        case MessageService msgService:
                            try
                            {
                                topMessage = msgService.action switch
                                {
                                    MessageActionPinMessage => "Сообщение закреплено",
                                    _ => "Неизвестное действие"
                                };
                            }
                            catch { topMessage = "msg service exception"; }
                            break;
                    }
                    DialogInfo? tmp = dialogs.UserOrChat(dialog) switch
                    {
                        User user => new(user, await LoadedAccountsWorker.DownloadAvatar(account.Client, user), topMessage, dialog.TopMessage),
                        Chat chat => new(chat, await LoadedAccountsWorker.DownloadAvatar(account.Client, chat), topMessage, dialog.TopMessage),
                        Channel channel => new(channel, await LoadedAccountsWorker.DownloadAvatar(account.Client, channel), topMessage, dialog.TopMessage),
                        _ => null,
                    };
                    if (tmp?.GetInputPeer() != null)
                    {
                        offsetPeer = tmp.GetInputPeer();
                        offsetDate = dialogs.Messages.First(d => d.ID == dialog.TopMessage).Date;
                        offsetId = dialog.TopMessage;
                    }
                    if (tmp != null && !tmp.IsBot() && !tmp.IsChannel())
                    {
                        tmp.IsTarget = targets.FirstOrDefault(g => g.PeerId == tmp.PeerId)?.LastTargetLog.First().Status == LoggingTargetStatus.Enabled;
                        result.Add(tmp);
                    }
                    if (result.Count >= limit) break;
                }
            }
            return result;
        }
    }
}
