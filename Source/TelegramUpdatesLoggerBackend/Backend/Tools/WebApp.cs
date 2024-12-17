using Backend.Tools.Structs;
using Config;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;

namespace Backend.Tools
{
    public class WebApp
    {
        public User? User { get; private set; } = null;
        public DateTime? AuthDate { get; private set; } = null;
        public WebApp(string initData)
        {
            var data = HttpUtility.ParseQueryString(initData);

            var sortedData = new SortedDictionary<string, string>(
                data.AllKeys
                    .Where(k => k != null)
                    .ToDictionary(k => k ?? "", k => data[k] ?? ""),
                    StringComparer.Ordinal);

            string dataCheckString = string.Join(
                "\n",
                sortedData
                    .Where(pair => pair.Key != "hash")
                    .Select(pair => $"{pair.Key}={pair.Value}"));

            var secretKey = HMACSHA256.HashData(
                    Encoding.UTF8.GetBytes("WebAppData"),
                    Encoding.UTF8.GetBytes(ProgramConfig.TelegramBotKey));

            string reference = data.Get("hash") ?? "";

            string resultHash = Convert.ToHexString(
                HMACSHA256.HashData(
                    secretKey,
                    Encoding.UTF8.GetBytes(dataCheckString)));

            if (!resultHash.Equals(reference, StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("Wrong hash in initData");

            var tmpUser = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(data.Get("user") ?? "{}");
            if (tmpUser != null)
                User = new User(tmpUser);
            if (int.TryParse(data.Get("auth_date"), out int auth_date))
                AuthDate = TimeStampConvertor.IntToDatetime(auth_date);
        }
    }
}
