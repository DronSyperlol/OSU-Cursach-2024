using Config;
using Core.Interfaces;
using Core.Types;
using Database;
using WTelegram;

namespace Core.Workers
{
    public class AccountManager : IWorker
    {
        static readonly List<LoadedAccount> LoadedAccounts = [];

        async Task IWorker.Handle(ApplicationContext context)
        {
            var triggerDate = DateTime.UtcNow.AddMinutes(-5);
            var accountToDispose = LoadedAccounts.Where(la => la.IsActive == false && la.LastTrigger < triggerDate);
            while(accountToDispose.Any())
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
            => new( ProgramConfig.TelegramApiAuth.ApiId,
                    ProgramConfig.TelegramApiAuth.ApiHash,
                    ProgramConfig.TelegramApiAuth.SessionsDir + $"{userId}_{phoneNumber}");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="userId"></param>
        /// <returns>Status</returns>
        /// <exception cref="Exception"></exception>
        public static async Task<string> OpenNewAccount(string phoneNumber, long userId)
        {
            LoadedAccount? account = LoadedAccounts.FirstOrDefault(la => la.PhoneNumber == phoneNumber);
            if (account != null)
            {
                await account.Client.DisposeAsync();
                account.Client = GetNewClient(userId, phoneNumber);
            }
            else
            {
                account = new LoadedAccount()
                {
                    Client = GetNewClient(userId, phoneNumber),
                    PhoneNumber = phoneNumber,
                    OwnerId = userId
                };
                LoadedAccounts.Add(account);
            }
            account.Status = await account.Client.Login(phoneNumber);
            if (account.Status != "verification_code")
                throw new Exception("Account must be registered!");
            account.Trigger();
            return account.Status;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="code"></param>
        /// <returns>Status</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<string> SetCodeToNewAccount(string phoneNumber, string code)
        {
            LoadedAccount? account = LoadedAccounts.FirstOrDefault(la => la.PhoneNumber == phoneNumber)
                ?? throw new ArgumentNullException($"Can't find account by phone number {phoneNumber}");
            if (account.Status != "verification_code") 
                throw new InvalidOperationException("Status must be \"verification_code\"");
            account.Status = await account.Client.Login(code);
            if (account.Status == "" && account.Client.UserId != 0)
            {
                account.Status = "Logged";
                account.IsActive = true;
            }
            account.Trigger();
            return account.Status;
        }

        public static async Task<string> SetCloudPasswordToNewAccount(string phoneNumber, string password)
        {
            LoadedAccount? account = LoadedAccounts.FirstOrDefault(la => la.PhoneNumber == phoneNumber)
                ?? throw new ArgumentNullException($"Can't find account by phone number {phoneNumber}");
            if (account.Status != "password")
                throw new InvalidOperationException("Status must be \"password\"");
            account.Status = await account.Client.Login(password);
            if (account.Status == "" && account.Client.UserId != 0)
            {
                account.Status = "Logged";
                account.IsActive = true;
            }
            account.Trigger();
            return account.Status;
        }
    }
}
