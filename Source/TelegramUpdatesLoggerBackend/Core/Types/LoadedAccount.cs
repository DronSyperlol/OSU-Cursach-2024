using TL;
using WTelegram;

namespace Core.Types
{
    internal class LoadedAccount
    {
        public required Client Client { get; set; }
        public required string PhoneNumber { get; set; } // Uses as unique id
        public string Status { get; set; } = "";
        public DateTime LoginTime { get; set; } = DateTime.UtcNow;
        public DateTime LastTrigger { get; set; } = DateTime.UtcNow;
        public required long OwnerId { get; set; }
        public bool InUse { get; set; } = false;
        public long IdInDb { get; set; } = 0;
        public void Trigger()
        {
            LastTrigger = DateTime.UtcNow;
        }

        public struct Statuses
        {
            public static readonly string Unknown = "Unknown";
            public static readonly string Logged = "Logged in";
            public static readonly string Code = "verification_code";
            public static readonly string Password = "password";
        }
    }
}
