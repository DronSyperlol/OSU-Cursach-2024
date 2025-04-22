using Config;
using Core.Types;
using Core.Workers;
using Database;
using Database.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
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
        public static async Task<List<DialogInfo>> GetDialogs(this LoadedAccount account, int offsetId, int limit, CancellationToken cancellationToken)
        {
            using var context = new ApplicationContext();
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
                        User user => new(user, await AccountManager.DownloadAvatar(account.Client, user), topMessage, dialog.TopMessage),
                        Chat chat => new(chat, await AccountManager.DownloadAvatar(account.Client, chat), topMessage, dialog.TopMessage),
                        Channel channel => new(channel, await AccountManager.DownloadAvatar(account.Client, channel), topMessage, dialog.TopMessage),
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


        public static async Task<List<object>> GetDialogHistory(this LoadedAccount account, InputPeer inputPeer, int offsetId, int limit)
        {
            try
            {
                //var dialogs = await account.Client.Messages_GetDialogs(offset_id: peerId, limit: 1);
                //var inputPeer = dialogs.UserOrChat(dialogs.Dialogs.First()).ToInputPeer();
                //var history = await account.Client.Messages_GetHistory(inputPeer, offsetId, limit: limit);

                var history = await account.Client.Messages_GetHistory(inputPeer, offsetId, limit: limit);
                Console.WriteLine(history.Count);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            throw new NotImplementedException();
        }

        public static InputPeer GetInputPeer(long peerId, long? accessHash)
        {
            if (peerId > 0) return new InputPeerUser(peerId, accessHash ?? 0);
            else if (accessHash == null) return new InputPeerChat(peerId);
            else return new InputPeerChannel(peerId, accessHash ?? 0);
        }
    }
}
