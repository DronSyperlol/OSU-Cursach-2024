using Database.Enum;

namespace Database.Entities
{
    public class Session
    {
        public ulong Id { get; set; }
        public required string Code { get; set; }
        public long ToUserId { get; set; }
        public required User ToUser { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DiedAt { get; set; }
        public SessionStatus Status { get; set; }
    }
}
