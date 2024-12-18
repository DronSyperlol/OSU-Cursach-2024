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
        public string SessionFileName { get; set; }
        public User Owner { get; set; }
        public AccountStatus Status { get; set; }
    }
}
