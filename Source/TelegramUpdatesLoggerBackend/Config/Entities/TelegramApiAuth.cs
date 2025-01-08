namespace Config.Entities
{
    public class TelegramApiAuth
    {
        public TelegramApiAuth(int apiId, string apiHash)
        {
            ApiId = apiId;
            ApiHash = apiHash;
        }
        public int ApiId { get; set; }
        public string ApiHash { get; set; }
        public string SessionsDir { get; set; } = "/app/sessions/";
    }
}
