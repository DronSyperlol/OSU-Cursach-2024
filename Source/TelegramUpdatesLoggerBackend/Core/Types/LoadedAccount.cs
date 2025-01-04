using WTelegram;

namespace Core.Types
{
    internal class LoadedAccount
    {
        public required Client Client { get; set; }
        public required string PhoneNumber { get; set; } // Uses as unique id
        public string Status { get; set; } = "";
        public DateTime LastTrigger { get; set; } = DateTime.UtcNow;
        public void Trigger()
        {
            LastTrigger = DateTime.UtcNow;
        }
    }
}
