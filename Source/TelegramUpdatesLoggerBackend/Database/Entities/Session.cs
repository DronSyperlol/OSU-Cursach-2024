using Database.Enum;

namespace Database.Entities
{
    public class Session
    {
        public ulong Id { get; set; }
        public string Code { get; set; }
        public User ToUser { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DiedAt { get; set; }
        public SessionStatus Status { get; set; }
    }
}
