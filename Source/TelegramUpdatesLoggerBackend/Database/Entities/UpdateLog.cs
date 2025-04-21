namespace Database.Entities
{
    public abstract class UpdateLog
    {
        public long Id { get; set; }
        //public UpdateType Type { get; set; } // Не понадобится (скорее всего)
        public DateTime Time { get; set; }
        public long LoggingTargetId { get; set; }
        public required LoggingTarget LoggingTarget { get; set; }
    }
}
