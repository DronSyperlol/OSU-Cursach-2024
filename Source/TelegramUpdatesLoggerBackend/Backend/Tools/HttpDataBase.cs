#pragma warning disable IDE1006

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Backend.Tools
{
    public abstract class HttpDataBase : ParsebleToDictionaryBase
    {
        public string? signature { get; private set; }
        public long ts { get; private set; }
        public void Sign(long userId, string sessionCode)
        {
            var sortedParams = GetSortedParams();
            ts = TimeStampConvertor.DatetimeToLong(DateTime.UtcNow);
            string dataStr = $"userId:{userId}_ts:{ts}_";
            dataStr += string.Join("&", sortedParams.Select(item => $"data.{item.Key}={item.Value}"));
            signature = Convert.ToHexString(HMACSHA256.HashData(
                Encoding.UTF8.GetBytes(sessionCode),
                Encoding.UTF8.GetBytes(dataStr)))!;
        }

        public void Verify(long userId, string sessionCode)
        {
            if (sessionCode == "_") throw new InvalidOperationException(nameof(sessionCode) + " is empty");
            if (signature == null) throw new InvalidOperationException(nameof(signature) + " is null");
            var sortedParams = GetSortedParams();
            string dataStr = $"userId:{userId}_ts:{TimeStampConvertor.DatetimeToLong(DateTime.UtcNow)}_";
            dataStr += string.Join("&", sortedParams.Select(item => $"data.{item.Key}={item.Value}"));
            string tmp = Convert.ToHexString(HMACSHA256.HashData(
                Encoding.UTF8.GetBytes(sessionCode),
                Encoding.UTF8.GetBytes(dataStr)))!;
            if (!signature.Equals(tmp)) throw new UnauthorizedAccessException("Signature do not match!");
        }

        //  В GetSortedParams надо ключи заполнять с именем корневого объекта.
        //  Пример: { root: { child: "value" } } -> { root.child : "value" }
        public virtual SortedDictionary<string, object> GetSortedParams()
        {
            var data = ToDict();
            var ret = new SortedDictionary<string, object>();
            DefaulTransform(data, "", ref ret);
            return ret;
        }

        static void DefaulTransform(
            Dictionary<string, object> source,
            string parentKey,
            ref SortedDictionary<string, object> result)
        {
            foreach (var item in source)
            {
                string currentKey = parentKey != string.Empty ? $"{parentKey}.{item.Key}" : item.Key;
                if (item.Value is Dictionary<string, object> dictionary)
                    DefaulTransform(dictionary, currentKey, ref result);
                else
                {
                    try
                    {
                        var tmp = ((JsonElement)item.Value).Deserialize<Dictionary<string, object>>() ?? [];
                        DefaulTransform(tmp, currentKey, ref result);
                    }
                    catch
                    {
                        result.Add(currentKey, item.Value);
                    }
                }
            }
        }
    }
}
