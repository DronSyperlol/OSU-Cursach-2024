using System.Text.Json;

namespace Backend.Tools.Structs
{
    public readonly struct User
    {
        public User(Dictionary<string, JsonElement> dict)
        {
            if (!dict.TryGetValue("id", out JsonElement tmp))
                throw new ArgumentException($"{nameof(dict)} must have field \"id\"");
            if (!tmp.TryGetInt64(out long id))
                throw new ArgumentException($"field \"id\" in {nameof(dict)} must be int64");
            ArgumentOutOfRangeException.ThrowIfNegative(id);
            if (!dict.TryGetValue("first_name", out tmp))
                throw new ArgumentException($"{nameof(dict)} must have field \"first_name\"");
            string? first_name = tmp.ToString();
            ArgumentNullException.ThrowIfNull(first_name);
            Id = Convert.ToInt64(id);
            FirstName = first_name;
            if (dict.TryGetValue("last_name", out tmp)) LastName = tmp.ToString();
            if (dict.TryGetValue("username", out tmp)) Username = tmp.ToString();
            if (dict.TryGetValue("language_code", out tmp)) LanguageCode = tmp.ToString();
            if (dict.TryGetValue("allows_write_to_pm", out tmp)) AllowsWriteToPm = tmp.GetBoolean();
            if (dict.TryGetValue("photo_url", out tmp)) PhotoUrl = tmp.ToString();
        }

        public long Id { get; } = 0;
        public string FirstName { get; } = "";
        public string? LastName { get; } = null;
        public string? Username { get; } = null;
        public string? LanguageCode { get; } = null;
        public bool AllowsWriteToPm { get; } = false;
        public string? PhotoUrl { get; } = null;
    }
}

