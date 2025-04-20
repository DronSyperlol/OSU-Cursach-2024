namespace Core.Workers.Types
{
    class Session
    {
        public Session(Database.Entities.Session source) 
        { 
            Id = source.Id;
            Code = source.Code;
            ToUserId = source.ToUser.Id;
            Trigger();
        }
        public ulong Id { get; set; }
        public string Code { get; set; }
        public long ToUserId { get; set; }
        public DateTime LastTrigger { get; private set; }

        public void Trigger()
        {
            LastTrigger = DateTime.UtcNow;
        }
    }
}
