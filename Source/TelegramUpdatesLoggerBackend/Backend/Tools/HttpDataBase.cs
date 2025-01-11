#pragma warning disable IDE1006

using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Backend.Tools
{
    public abstract class HttpDataBase : ParsebleToDictionaryBase
    {
        public string? signature { get; set; }
        public long ts { get; set; }
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
            sortedParams.Remove("signature");
            sortedParams.Remove("ts");
            string dataStr = $"userId:{userId}_ts:{ts}_";
            dataStr += string.Join("&", sortedParams.Select(item => $"data.{item.Key}={item.Value}"));
            string tmp = Convert.ToHexString(HMACSHA256.HashData(
                Encoding.UTF8.GetBytes(sessionCode),
                Encoding.UTF8.GetBytes(dataStr)))!;
            if (!signature.Equals(tmp, StringComparison.OrdinalIgnoreCase)) throw new UnauthorizedAccessException("Signature do not match!");
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
                else if (item.Value is IList objects)
                {
                    var dict = new Dictionary<string, object>();
                    int index = 0;
                    foreach (var obj in objects)
                    {
                        dict.Add(index++.ToString(), obj);
                    }
                    DefaulTransform(dict, currentKey, ref result);
                }
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
