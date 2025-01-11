using TL;
using WTelegram;

namespace Core.Types
{
    internal class LoadedAccount
    {
        public required Client Client { get; set; }
        public required string PhoneNumber { get; set; } // Uses as unique id
        public string Status { get; set; } = "";
        public DateTime LastTrigger { get; set; } = DateTime.UtcNow;
        public required long OwnerId { get; set; }
        public bool IsActive { get; set; } = false;
        public long IdInDb { get; set; } = 0;
        public void Trigger()
        {
            LastTrigger = DateTime.UtcNow;
        }

        public struct Statuses
        {
            public const string Unknown = "Unknown";
            public const string Logged = "Logged in";
            public const string Code = "verification_code";
            public const string Password = "password";
        }
    }
}
