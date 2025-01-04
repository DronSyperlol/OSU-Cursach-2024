using Config;
using Core.Interfaces;
using Core.Types;
using Database;
using System.Formats.Asn1;
using System.Linq;
using System.Runtime.CompilerServices;
using TL.Methods;

namespace Core.Workers
{
    public class AccountManager : IWorker
    {
        static readonly List<LoadedAccount> LoadedAccounts = [];

        Task IWorker.Handle(ApplicationContext context)
        {
            throw new NotImplementedException();
        }
        
        public async Task OpenNewAccount(string phoneNumber)
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
                };
                LoadedAccounts.Add(account);
            }
            account.Status = await account.Client.Login(phoneNumber);
            if (account.Status != "verification_code")
                throw new Exception("Account must be registered!");
            account.Trigger();
        }

        public async Task SetCodeToNewAccount(string phoneNumber, string code)
        {
            LoadedAccount? account = LoadedAccounts.FirstOrDefault(la => la.PhoneNumber == phoneNumber)
                ?? throw new ArgumentNullException($"Can't find account by phone number {phoneNumber}");
            if (account.Status != "verification_code") 
                throw new InvalidOperationException("Status must be \"verification_code\"");
            account.Status = await account.Client.Login(phoneNumber);
            account.Trigger();
        }

        public async Task SetCloudPasswordToNewAccount(string phoneNumber, string password)
        {

        }
    }
}
