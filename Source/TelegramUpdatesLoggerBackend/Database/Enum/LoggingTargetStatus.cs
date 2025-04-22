namespace Database.Enum
{
    public enum LoggingTargetStatus
    {
        Failure = -1,
        Unknown = 0,
        Enabled = 1, 
        Disabled = 2,

        Deleted = 10,
        Banned = 11,
    }
}
