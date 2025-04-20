using Database.Enum;

namespace Database.Entities
{
    public abstract class UpdateLog
    {
        public long Id { get; set; }
        public UpdateType Type { get; set; }
        public DateTime Time { get; set; }
        public long FromAccountId { get; set; }
        public required Account FromAccount { get; set; }
    }
}
