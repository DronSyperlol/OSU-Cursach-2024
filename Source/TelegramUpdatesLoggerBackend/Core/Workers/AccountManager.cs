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
            var accountToDispose = LoadedAccounts.Where(la => la.InUse == false && la.LastTrigger < triggerDate);
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
            => new(ProgramConfig.TelegramApiAuth.ApiId,
                    ProgramConfig.TelegramApiAuth.ApiHash,
                    ProgramConfig.TelegramApiAuth.SessionsDir + $"{userId}_{phoneNumber}");
        static Client GetRegisteredClient(long userId, string phoneNumber)
            => new((what) => what switch
            {
                "api_id" => ProgramConfig.TelegramApiAuth.ApiId.ToString(),
                "api_hash" => ProgramConfig.TelegramApiAuth.ApiHash,
                "phone_number" => phoneNumber,
                "session_pathname" => ProgramConfig.TelegramApiAuth.SessionsDir + $"{userId}_{phoneNumber}",
                _ => null
            });

        static async Task<LoadedAccount> StarterNew(long userId, string phoneNumber)
        {
            var lAcc = await Starter(GetNewClient, userId, phoneNumber, true);
            await lAcc.Client.ConnectAsync();
            lAcc.Status = LoadedAccount.Statuses.Unknown;
            return lAcc;
        }
        static async Task<LoadedAccount> StarterExist(long userId, string phoneNumber, bool canUpdate = false)
        {
            var lAcc = await Starter(GetRegisteredClient, userId, phoneNumber, canUpdate);
            try 
            { 
                var task = lAcc.Client.LoginUserIfNeeded();
                task.Wait(10_000);
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }
            if (canUpdate || lAcc.Status == "")
            {
                lAcc.Status = lAcc.Client.User == null ? LoadedAccount.Statuses.Unknown : LoadedAccount.Statuses.Logged;
            }
            return lAcc;
        }
        static async Task<LoadedAccount> Starter(Func<long, string, Client> getClient, long userId, string phoneNumber, bool canUpdate)
        {
            LoadedAccount? lAcc = LoadedAccounts.FirstOrDefault(la => la.PhoneNumber == phoneNumber);
            if (lAcc == null)
            {
                lAcc = new LoadedAccount()
                {
                    Client = getClient(userId, phoneNumber),
                    PhoneNumber = phoneNumber,
                    OwnerId = userId,
                };
                LoadedAccounts.Add(lAcc);
            }
            else if (canUpdate)
            {
                await lAcc.Client.DisposeAsync();
                lAcc.Client = getClient(userId, phoneNumber);
            }
            return lAcc;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="userId"></param>
        /// <returns>Status</returns>
        /// <exception cref="AccessViolationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static async Task<string> OpenNewAccount(
            ApplicationContext context,
            string phoneNumber,
            long userId)
        {
            LoadedAccount? lAcc = null;
            var dbAccount = await context.Accounts
                .Include(a => a.AccountLogs)
                .FirstOrDefaultAsync(a => a.PhoneNumber == phoneNumber);
            if (dbAccount != null)
            {
                if (dbAccount.OwnerId != userId)
                    throw new AccessViolationException(
                        "You cannot log in to this account because it belongs to another user");
                lAcc = await StarterExist(userId, phoneNumber);
                lAcc.InUse = true;
                if (lAcc.Status == LoadedAccount.Statuses.Logged) // Аккаунт существует, повторный вход не нужен.
                {
                    if (dbAccount.Status != Database.Enum.AccountStatus.Active)
                    {
                        await AccountDbLog(
                            context,
                            dbAccount,
                            Database.Enum.AccountStatus.Active,
                            Database.Enum.AccountLogType.Login,
                            "Login when try login repeate");
                    }
                    lAcc.InUse = false;
                    return lAcc.Status;
                }
                else
                {
                    await AccountDbLog(
                        context,
                        dbAccount,
                        Database.Enum.AccountStatus.Broken,
                        Database.Enum.AccountLogType.Broke,
                        $"Can't start registered account at {DateTime.UtcNow}");
                }
            }
            lAcc = await StarterNew(userId, phoneNumber);
            User user = new() { Id = userId };
            context.Users.Attach(user);
            if (dbAccount == null)
            {
                dbAccount = new()
                {
                    Owner = user,
                    PhoneNumber = phoneNumber,
                    AccountLogs = [],
                };
                await context.Accounts.AddAsync(dbAccount);
            }
            await AccountDbLog(context, dbAccount, Database.Enum.AccountStatus.Opening, Database.Enum.AccountLogType.OpenNew, "", user);
            lAcc.IdInDb = dbAccount.Id;
            try
            {
                lAcc.Status = await lAcc.Client.Login(phoneNumber);
            }
            catch (Exception ex)
            {
                await AccountDbLog(context, dbAccount, Database.Enum.AccountStatus.Broken, Database.Enum.AccountLogType.Broke, ex.Message[..512], user);
            }
            if (lAcc.Status != LoadedAccount.Statuses.Code)
            {
                string errorMessage = $"CurrentStatus: {lAcc.Status} but expected \"{LoadedAccount.Statuses.Code}\"";
                await AccountDbLog(context, dbAccount, Database.Enum.AccountStatus.Broken, Database.Enum.AccountLogType.Broke, errorMessage, user);
                lAcc.InUse = false;
                throw new ArgumentException(errorMessage);
            }
            lAcc.Trigger();
            lAcc.InUse = false;
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
            lAcc.InUse = true;
            if (lAcc.Status != LoadedAccount.Statuses.Code)
            {
                lAcc.InUse = false;
                throw new InvalidOperationException($"Status must be \"{LoadedAccount.Statuses.Code}\"");
            }
            var dbAccount = await context.Accounts
                .Include(a => a.AccountLogs)
                .FirstOrDefaultAsync(a => a.PhoneNumber == phoneNumber);
            if (dbAccount == null)
            {
                lAcc.InUse = false;
                throw new ArgumentNullException($"Can't find account in DB by phone number {phoneNumber}");
            }
            User user = new() { Id = lAcc.OwnerId };
            context.Users.Attach(user);
            await AccountDbLog(context, dbAccount, Database.Enum.AccountStatus.Opening, Database.Enum.AccountLogType.SetCode, "", user);
            await context.SaveChangesAsync();
            try
            {
                lAcc.Status = await lAcc.Client.Login(code);
            }
            catch (Exception ex)
            {
                await AccountDbLog(context, dbAccount, Database.Enum.AccountStatus.Broken, Database.Enum.AccountLogType.Broke, ex.Message[..512], user);
            }
            if (lAcc.Client.User != null)
            {
                await AccountDbLog(
                    context,
                    dbAccount,
                    Database.Enum.AccountStatus.Active,
                    Database.Enum.AccountLogType.Login,
                    "Login after set code");
                lAcc.Status = LoadedAccount.Statuses.Logged;
            }
            lAcc.Trigger();
            lAcc.InUse = false;
            return lAcc.Status;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="password"></param>
        /// <returns>Status</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<string> SetCloudPasswordToNewAccount(
            ApplicationContext context,
            string phoneNumber,
            string password)
        {
            LoadedAccount? lAcc = LoadedAccounts.FirstOrDefault(la => la.PhoneNumber == phoneNumber)
                ?? throw new ArgumentNullException($"Can't find account by phone number {phoneNumber}");
            lAcc.InUse = true;
            if (lAcc.Status != LoadedAccount.Statuses.Password)
            {
                lAcc.InUse = false;
                throw new InvalidOperationException($"Status must be \"{LoadedAccount.Statuses.Password}\"");
            }
            var dbAccount = await context.Accounts
                .Include(a => a.AccountLogs)
                .FirstOrDefaultAsync(a => a.PhoneNumber == phoneNumber);
            if (dbAccount == null)
            {
                lAcc.InUse = false;
                throw new ArgumentNullException($"Can't find account in DB by phone number {phoneNumber}");
            }
            User user = new() { Id = lAcc.OwnerId };
            context.Users.Attach(user);
            await AccountDbLog(context, dbAccount, Database.Enum.AccountStatus.Opening, Database.Enum.AccountLogType.SetPass, "", user);
            try
            {
                lAcc.Status = await lAcc.Client.Login(password);
            }
            catch (Exception ex)
            {
                await AccountDbLog(context, dbAccount, Database.Enum.AccountStatus.Broken, Database.Enum.AccountLogType.Broke, ex.Message[..512], user);
            }
            if (lAcc.Client.User != null)
            {
                await AccountDbLog(
                    context,
                    dbAccount,
                    Database.Enum.AccountStatus.Active,
                    Database.Enum.AccountLogType.Login,
                    "Login after set code");
                lAcc.Status = LoadedAccount.Statuses.Logged;
            }
            lAcc.Trigger();
            lAcc.InUse = false;
            return lAcc.Status;
        }

        /// <summary>
        /// This method writes to the database
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dbAccount"></param>
        /// <param name="generalStatus"></param>
        /// <param name="logType"></param>
        /// <param name="logDescription"></param>
        /// <param name="ByUser"></param>
        /// <returns></returns>
        static async Task AccountDbLog(
            ApplicationContext context,
            Database.Entities.Account dbAccount,
            Database.Enum.AccountStatus generalStatus,
            Database.Enum.AccountLogType logType,
            string logDescription,
            User? ByUser = null)
        {
            dbAccount.Status = generalStatus;
            dbAccount.AccountLogs ??= [];
            dbAccount.AccountLogs.Add(new()
            {
                Account = dbAccount,
                Type = logType,
                Time = DateTime.UtcNow,
                ByUser = ByUser,
                Description = logDescription,
            });
            await context.SaveChangesAsync();
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

        public static async Task<List<DialogInfo>> GetDialogs(long userId, string phoneNumber, int offsetId, int limit)
        {
            const int maxMessagePreview = 50;
            LoadedAccount account = await StarterExist(userId, phoneNumber);
            var dialogs = await account.Client.Messages_GetDialogs(offset_id: offsetId, limit: limit);
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
                                TopMessage = message.message.Length > maxMessagePreview ? message.message[..(maxMessagePreview - 3)] + "..." : message.message,
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
                                TopMessage = message.message.Length > maxMessagePreview ? message.message[..(maxMessagePreview - 3)] + "..." : message.message,
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
                                TopMessage = message.message.Length > maxMessagePreview ? message.message[..(maxMessagePreview - 3)] + "..." : message.message,
                                DialogType = DialogInfo.Types.Channel,
                                PhotoUrl = ProgramConfig.Path.Static + photoFilename
                            });
                        }
                        break;
                }
            }
            return result;
        }

        public static async Task<AccountInfo?> GetMe(long userId, string phoneNumber)
        {
            LoadedAccount account = await StarterExist(userId, phoneNumber);
            if (account.Status == LoadedAccount.Statuses.Logged)
            {
                return new()
                {
                    PhoneNumber = phoneNumber,
                    PhotoUrl = ProgramConfig.Path.Static + await DownloadAvatar(account.Client, account.Client.User),
                    Title = account.Client.User.first_name + " " + account.Client.User.last_name,
                    Username = account.Client.User.username
                };
            }
            else return null;
        }
    }
}
