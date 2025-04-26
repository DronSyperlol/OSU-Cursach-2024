using Database.Enum;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Database.Entities
{
    [Index(nameof(PeerId))]
    public class LoggingTarget
    {
        public long Id { get; set; }
        public long FromAccountId { get; set; }
        public required Account FromAccount { get; set; }
        public required long PeerId { get; set; }
        public required long? AccessHash { get; set; }
        public DateTime Time { get; set; } = DateTime.UtcNow;
        public LoggingTargetType Type { get; set; }
        public LoggingTargetStatus Status { get; set; }
        public List<UpdateLog> Logs { get; set; } = [];
        [MaxLength(100)]
        public string? Error { get; set; } = null; // not null when status is Failure (-1)

        public long? PrevTargetId { get; set; } = null;
        public LoggingTarget? PrevTarget { get; set; } = null;
    }
}
