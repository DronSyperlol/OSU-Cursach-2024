using Config;
using Core.Interfaces;
using Core.Types;
using Database;

namespace Core.Workers
{
    public class AccountManager : IWorker
    {
        static readonly List<LoadedAccount> LoadedAccounts = [];

        Task IWorker.Handle(ApplicationContext context)
        {
            throw new NotImplementedException();
        }
        
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
                account.Client = new WTelegram.Client(ProgramConfig.TelegramApiAuth.ApiId, ProgramConfig.TelegramApiAuth.ApiHash);
            }
            else
            {
                account = new LoadedAccount()
                {
                    Client = new WTelegram.Client(ProgramConfig.TelegramApiAuth.ApiId, ProgramConfig.TelegramApiAuth.ApiHash),
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
            Console.WriteLine("Current status:" + account.Status);
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
            Console.WriteLine("Current status:" + account.Status);
            account.Trigger();
            return account.Status;
        }
    }
}
