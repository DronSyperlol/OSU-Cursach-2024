using Database.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    public class Account
    {
        public long Id { get; set; }
        [NotMapped]
        public long TelegramId { get; set; } // reserved
        [MaxLength(50)]
        public required string PhoneNumber { get; set; }
        public required User Owner { get; set; }
        public long OwnerId { get; set; }
        public AccountStatus Status { get; set; }
        public List<AccountLog>? AccountLogs { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
