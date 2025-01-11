using Config;
using Core.Interfaces;
using Core.Types;
using Database;
using Microsoft.EntityFrameworkCore;
using WTelegram;
using TL;
using User = Database.Entities.User;

namespace Core.Workers
{
    public class AccountManager : IWorker
    {
        static readonly List<LoadedAccount> LoadedAccounts = [];

        async Task IWorker.Handle(ApplicationContext context)
        {
            var triggerDate = DateTime.UtcNow.AddMinutes(-5);
            var accountToDispose = LoadedAccounts.Where(la => la.IsActive == false && la.LastTrigger < triggerDate);
            while (accountToDispose.Any())
            {
                var account = accountToDispose.FirstOrDefault();
                if (account != null)
                {
                    await account.Client.DisposeAsync();
                    LoadedAccounts.Remove(account);
                    Console.WriteLine($"account {account.PhoneNumber} removed by inactive");
                }
            }
        }

        static Client GetNewClient(long userId, string phoneNumber)
            => new((what) => what switch
            {
                "api_id" => ProgramConfig.TelegramApiAuth.ApiId.ToString(),
                "api_hash" => ProgramConfig.TelegramApiAuth.ApiHash,
                "phone_number" => phoneNumber,
                _ => null
            });
            //=> new(ProgramConfig.TelegramApiAuth.ApiId,
            //        ProgramConfig.TelegramApiAuth.ApiHash,
            //        ProgramConfig.TelegramApiAuth.SessionsDir + $"{userId}_{phoneNumber}");

        static string get(string what)
        {
            return what switch
            {
                "sdf" => "sdgwe",
                "iowetioweut" => "kjasdgjk",
                _ => "",
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="userId"></param>
        /// <returns>Status</returns>
        /// <exception cref="Exception"></exception>
        public static async Task<string> OpenNewAccount(
            ApplicationContext context,
            string phoneNumber,
            long userId)
        {
            LoadedAccount lAcc = await Starter(userId, phoneNumber, true);
            User owner = new() { Id = userId };
            context.Users.Attach(owner);
            var dbAccount = await context.Accounts
                .Include(a => a.AccountLogs)
                .FirstOrDefaultAsync(a => a.PhoneNumber == phoneNumber);
            if (dbAccount == null)
            {
                dbAccount = new()
                {
                    Owner = owner,
                    PhoneNumber = phoneNumber,
                    Status = Database.Enum.AccountStatus.Opening,
                    CreatedAt = DateTime.UtcNow,
                    AccountLogs = []
                };
                await context.Accounts.AddAsync(dbAccount);
            }
            else
            {
                dbAccount.Status = Database.Enum.AccountStatus.Opening;
            }
            lAcc.IdInDb = dbAccount.Id;
            dbAccount.AccountLogs!.Add(new()
            {
                Account = dbAccount,
                Type = Database.Enum.AccountLogType.OpenNew,
                Time = DateTime.UtcNow,
                ByUser = owner,
                Description = null,
            });
            await context.SaveChangesAsync();
            try
            {
                lAcc.Status = await lAcc.Client.Login(phoneNumber);
            }
            catch (Exception ex)
            {
                dbAccount.Status = Database.Enum.AccountStatus.Broken;
                dbAccount.AccountLogs.Add(new()
                {
                    Account = dbAccount,
                    Type = Database.Enum.AccountLogType.Broke,
                    Description = ex.Message[..512],
                    ByUser = owner,
                    Time = DateTime.UtcNow
                });
                await context.SaveChangesAsync();
            }
            if (lAcc.Client.User != null)
            {
                dbAccount.Status = Database.Enum.AccountStatus.Active;
                dbAccount.AccountLogs.Add(new()
                {
                    Account = dbAccount,
                    Type = Database.Enum.AccountLogType.Login,
                    ByUser = owner,
                    Time = DateTime.UtcNow
                });
                await context.SaveChangesAsync();
                lAcc.Status = "Logged";
                lAcc.IsActive = true;
            }
            else if (lAcc.Status != "verification_code")
            {
                throw new ArgumentException($"CurrentStatus: {lAcc.Status} but expected \"verification_code\"");
            }

            lAcc.Trigger();
            return lAcc.Status;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="code"></param>
        /// <returns>Status</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<string> SetCodeToNewAccount(
            ApplicationContext context,
            string phoneNumber,
            string code)
        {
            LoadedAccount? lAcc = LoadedAccounts.FirstOrDefault(la => la.PhoneNumber == phoneNumber)
                ?? throw new ArgumentNullException(
                    $"Can't find account by phone number {phoneNumber}");
            if (lAcc.Status != "verification_code")
                throw new InvalidOperationException("Status must be \"verification_code\"");
            var dbAccount = await context.Accounts
                .Include(a => a.AccountLogs)
                .FirstOrDefaultAsync(a => a.PhoneNumber == phoneNumber)
                ?? throw new ArgumentNullException(
                    $"Can't find account by phone number {phoneNumber}");
            User owner = new() { Id = lAcc.OwnerId };
            context.Users.Attach(owner);
            dbAccount.AccountLogs!.Add(new()
            {
                Account = dbAccount,
                ByUser = owner,
                Time = DateTime.UtcNow,
                Type = Database.Enum.AccountLogType.SetCode,
            });
            await context.SaveChangesAsync();
            try
            {
                lAcc.Status = await lAcc.Client.Login(code);
            }
            catch (Exception ex)
            {
                dbAccount.Status = Database.Enum.AccountStatus.Broken;
                dbAccount.AccountLogs.Add(new()
                {
                    Account = dbAccount,
                    Type = Database.Enum.AccountLogType.Broke,
                    Description = ex.Message[..512],
                    ByUser = owner,
                    Time = DateTime.UtcNow
                });
                await context.SaveChangesAsync();
            }
            if (lAcc.Client.User != null)
            {
                dbAccount.Status = Database.Enum.AccountStatus.Active;
                dbAccount.AccountLogs.Add(new()
                {
                    Account = dbAccount,
                    Type = Database.Enum.AccountLogType.Login,
                    ByUser = owner,
                    Time = DateTime.UtcNow
                });
                await context.SaveChangesAsync();
                lAcc.Status = "Logged";
                lAcc.IsActive = true;
            }
            lAcc.Trigger();
            return lAcc.Status;
        }

        public static async Task<string> SetCloudPasswordToNewAccount(
            ApplicationContext context,
            string phoneNumber,
            string password)
        {
            LoadedAccount? lAcc = LoadedAccounts.FirstOrDefault(la => la.PhoneNumber == phoneNumber)
                ?? throw new ArgumentNullException($"Can't find account by phone number {phoneNumber}");
            if (lAcc.Status != "password")
                throw new InvalidOperationException("Status must be \"password\"");
            var dbAccount = await context.Accounts
                .Include(a => a.AccountLogs)
                .FirstOrDefaultAsync(a => a.PhoneNumber == phoneNumber)
                ?? throw new ArgumentNullException(
                    $"Can't find account by phone number {phoneNumber}");
            User owner = new() { Id = lAcc.OwnerId };
            context.Users.Attach(owner);
            dbAccount.AccountLogs!.Add(new()
            {
                Account = dbAccount,
                ByUser = owner,
                Time = DateTime.UtcNow,
                Type = Database.Enum.AccountLogType.SetPass,
            });
            await context.SaveChangesAsync();
            try
            {
                lAcc.Status = await lAcc.Client.Login(password);
            }
            catch (Exception ex)
            {
                dbAccount.Status = Database.Enum.AccountStatus.Broken;
                dbAccount.AccountLogs.Add(new()
                {
                    Account = dbAccount,
                    Type = Database.Enum.AccountLogType.Broke,
                    Description = ex.Message[..512],
                    ByUser = owner,
                    Time = DateTime.UtcNow
                });
                await context.SaveChangesAsync();
            }
            if (lAcc.Client.User != null)
            {
                dbAccount.Status = Database.Enum.AccountStatus.Active;
                dbAccount.AccountLogs.Add(new()
                {
                    Account = dbAccount,
                    Type = Database.Enum.AccountLogType.Login,
                    ByUser = owner,
                    Time = DateTime.UtcNow
                });
                await context.SaveChangesAsync();
                lAcc.Status = "Logged in";
                lAcc.IsActive = true;
            }
            lAcc.Trigger();
            return lAcc.Status;
        }

        static async Task<LoadedAccount> Starter(long userId, string phoneNumber, bool canUpdate = false)
        {
            LoadedAccount? lAcc = LoadedAccounts.FirstOrDefault(la => la.PhoneNumber == phoneNumber);
            if (lAcc == null)
            {
                lAcc = new LoadedAccount()
                {
                    Client = GetNewClient(userId, phoneNumber),
                    PhoneNumber = phoneNumber,
                    OwnerId = userId,
                };
                LoadedAccounts.Add(lAcc);
                await lAcc.Client.LoginUserIfNeeded();
                lAcc.Status = lAcc.Client.User == null ? LoadedAccount.Statuses.Unknown : LoadedAccount.Statuses.Logged;
            }
            else if (canUpdate)
            {
                await lAcc.Client.DisposeAsync();
                lAcc.Client = GetNewClient(userId, phoneNumber);
                var me = await lAcc.Client.LoginUserIfNeeded();
                lAcc.Status = lAcc.Client.User == null ? LoadedAccount.Statuses.Unknown : LoadedAccount.Statuses.Logged;
            }
            return lAcc;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="peer"></param>
        /// <returns>Photo's filename</returns>
        static async Task<string> DownloadAvatar(Client client, IPeerInfo peer)
        {
            string filename = "avatar_" + peer.ID;
            using var fileStream = new FileStream(ProgramConfig.TelegramApiAuth.DownloadsDir + filename, FileMode.Create, FileAccess.ReadWrite);
            var photoInfo = await client.DownloadProfilePhotoAsync(peer, fileStream);
            fileStream.Close();
            return filename;
        }

        public static async Task<List<DialogInfo>> GetDialogs(long userId, string phoneNumber)
        {
            const int maxMessagePreview = 50;
            LoadedAccount account = await Starter(userId, phoneNumber);
            var dialogs = await account.Client.Messages_GetDialogs(limit:10);
            List<DialogInfo> result = [];
            foreach (var dialog in dialogs.Dialogs)
            {
                Message message = (Message)dialogs.Messages.First(m => m.Peer.ID == dialog.Peer.ID);
                switch (dialogs.UserOrChat(dialog))
                {
                    case TL.User user:
                        {
                            var photoFilename = await DownloadAvatar(account.Client, user);
                            result.Add(new()
                            {
                                PeerId = user.id,
                                Title = user.first_name + " " + user.last_name,
                                TopMessage = message.message.Length > maxMessagePreview ? message.message[..(maxMessagePreview-3)]+"..." : message.message,
                                DialogType = user.IsBot ? DialogInfo.Types.Bot : DialogInfo.Types.User,
                                PhotoUrl = ProgramConfig.Path.Static + photoFilename
                            });
                            
                        }
                        break;
                    case TL.Chat chat:
                        {
                            var photoFilename = await DownloadAvatar(account.Client, chat);
                            result.Add(new()
                            {
                                PeerId = chat.id,
                                Title = chat.Title,
                                TopMessage = message.message.Length > maxMessagePreview ? message.message[..(maxMessagePreview-3)]+"..." : message.message,
                                DialogType = DialogInfo.Types.Chat,
                                PhotoUrl = ProgramConfig.Path.Static + photoFilename
                            });
                        }
                        break;
                    case TL.Channel channel:
                        {
                            
                            var photoFilename = await DownloadAvatar(account.Client, channel);
                            result.Add(new()
                            {
                                PeerId = channel.id,
                                Title = channel.Title,
                                TopMessage = message.message.Length > maxMessagePreview ? message.message[..(maxMessagePreview-3)]+"..." : message.message,
                                DialogType = DialogInfo.Types.Channel,
                                PhotoUrl = ProgramConfig.Path.Static + photoFilename
                            });
                        }
                        break;
                }
            }   
            return result;
        }

        public static async Task<string> DownloadMyAvatar(long userId, string phoneNumber)
        {
            LoadedAccount account = await Starter(userId, phoneNumber);
            if (account.Status == LoadedAccount.Statuses.Logged) 
                return await DownloadAvatar(account.Client, account.Client.User);
            return "";
        }
    }
}
