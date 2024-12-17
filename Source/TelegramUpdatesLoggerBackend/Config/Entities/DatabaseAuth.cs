namespace Config.Entities
{
    public class DatabaseAuth
    {
        public string? Host { get; private set; }
        public string? User { get; private set; }
        public string? Password { get; private set; }
        public string? Database { get; private set; }

        public DatabaseAuth(string Host, string User, string Password, string Database, string Version)
        {
            this.Host = Host;
            this.User = User;
            this.Password = Password;
            this.Database = Database;
            var splited = Version.Split('.');
        }
        public string ConnectionString
        {
            get
            {
                return $"server={Host};" +
                       $"user={User};" +
                       $"password={Password};" +
                       $"database={Database}";
            }
        }
    }
}