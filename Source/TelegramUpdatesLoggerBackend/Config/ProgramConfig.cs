namespace Config
{
    public static class ProgramConfig
    {

        public static string TelegramBotKey { get; }
        static ProgramConfig()
        {
            TelegramBotKey = Environment.GetEnvironmentVariable("TELEGRAM_BOT_KEY") ?? "";

        }
    }
}
