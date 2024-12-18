using Config.Entities;

namespace Config
{
    public static class ProgramConfig
    {
        public static string TelegramBotKey { get; }
        public static DatabaseAuth DatabaseAuth { get; }

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

            DEV = Convert.ToBoolean(Environment.GetEnvironmentVariable("DEV"));
        }
    }
}
