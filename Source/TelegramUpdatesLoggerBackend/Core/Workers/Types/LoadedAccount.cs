using System.Net.Sockets;
using System.Threading.Tasks;
using TL.Methods;
using WTelegram;

namespace Core.Workers.Types
{
    public class LoadedAccount
    {
        public required Client Client { get; set; }
        public required string PhoneNumber { get; set; } // Uses as unique id
        public string Status { get; set; } = "";
        public DateTime LoginTime { get; set; } = DateTime.UtcNow;
        public DateTime LastTrigger { get; set; } = DateTime.UtcNow;
        public required long OwnerId { get; set; }
        public bool InUse { get; set; } = false;
        public long IdInDb { get; set; } = 0;

        public event Action? OnRestarted;

        public DateTime _lastRestart = DateTime.MinValue;

        public async Task Reconnect()
        {
            if (_lastRestart.AddSeconds(10) > DateTime.UtcNow)
                return;
            await Client.LoginUserIfNeeded();
            if (Client.User != null)
            {
                OnRestarted?.Invoke();
                Status = Statuses.Logged;
            }
        }

        public void Trigger()
        {
            LastTrigger = DateTime.UtcNow;
        }

        public readonly struct Statuses
        {
            public static readonly string Unknown = "Unknown";
            public static readonly string Logged = "Logged in";
            public static readonly string Code = "verification_code";
            public static readonly string Password = "password";
        }
    }
}
