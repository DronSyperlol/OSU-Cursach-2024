using Config.Entities;

namespace Config
{
    public static class ProgramConfig
    {
        public const int LoggingSaveSec = 5;

        public static string TelegramBotKey { get; }
        public static DatabaseAuth DatabaseAuth { get; }
        public static TelegramApiAuth TelegramApiAuth { get; }
        public static PathCollection Path { get; } 

        public static bool DEV { get; }

        static ProgramConfig()
        {
            TelegramBotKey = Environment.GetEnvironmentVariable("TELEGRAM_BOT_KEY") ?? "";

            DatabaseAuth = new(
                "telegram-logger-mysql",
                "root",
                Environment.GetEnvironmentVariable("MYSQL_ROOT_PASSWORD") ?? "root",
                "TelegramLogger",
                Environment.GetEnvironmentVariable("MYSQL_VERSION") ?? "1.0.0");

            TelegramApiAuth = new(
                int.Parse(Environment.GetEnvironmentVariable("API_ID") ?? "0"),
                Environment.GetEnvironmentVariable("API_HASH") ?? "");

            DEV = Convert.ToBoolean(Environment.GetEnvironmentVariable("DEV"));
            Path = new(Environment.GetEnvironmentVariable("BACKEND_FOREIGN_URL") ?? "~");
        }


    }
}
