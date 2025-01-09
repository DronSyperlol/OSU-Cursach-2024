using Database.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    public class AccountLog
    {
        public long Id { get; set; }
        public AccountLogType Type { get; set; }
        [MaxLength(512)]
        public string? Description { get; set; }
        public required Account Account { get; set; }
        public User? ByUser { get; set; }
        public DateTime Time { get; set; } = DateTime.UtcNow;
        [NotMapped]
        [MaxLength(512)]
        public string? RawData { get; set; } // reserved
    }
}
