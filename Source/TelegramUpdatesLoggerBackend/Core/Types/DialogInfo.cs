using Config;
using TL;

namespace Core.Types
{
    public class DialogInfo
    {
        public const int MAX_MESSAGE_PREVIEW = 50;

        public DialogInfo(User reference, string photoFilename, string topMessage)
        {
            PeerId = reference.id;
            AccessHash = reference.access_hash.ToString();
            Title = reference.first_name + " " + reference.last_name;
            TopMessage = topMessage.Length > MAX_MESSAGE_PREVIEW ? topMessage[..(MAX_MESSAGE_PREVIEW - 3)] + "..." : topMessage;
            DialogType = reference.IsBot ? Types.Bot : Types.User;
            PhotoUrl = ProgramConfig.Path.Static + photoFilename;
        }
        public DialogInfo(Chat reference, string photoFilename, string topMessage)
        {
            PeerId = reference.id;
            AccessHash = null;
            Title = reference.Title;
            TopMessage = topMessage.Length > MAX_MESSAGE_PREVIEW ? topMessage[..(MAX_MESSAGE_PREVIEW - 3)] + "..." : topMessage;
            DialogType = Types.Chat;
            PhotoUrl = ProgramConfig.Path.Static + photoFilename;
        }
        public DialogInfo(Channel reference, string photoFilename, string topMessage)
        {
            PeerId = reference.id;
            AccessHash = reference.access_hash.ToString();
            Title = reference.Title;
            TopMessage = topMessage.Length > MAX_MESSAGE_PREVIEW ? topMessage[..(MAX_MESSAGE_PREVIEW - 3)] + "..." : topMessage;
            DialogType = Types.Channel;
            PhotoUrl = ProgramConfig.Path.Static + photoFilename;
        }

        public long PeerId { get; set; }
        public string? AccessHash { get; set; }
        public string Title { get; set; }
        public string TopMessage { get; set; }
        public bool IsTarget { get; set; }
        public string DialogType { get; set; } 
        public string PhotoUrl { get; set; }

        public struct Types
        {
            public const string User = "User";
            public const string Chat = "Chat";
            public const string Channel = "Channel";
            public const string Bot = "User|Bot";
        }

        public override string ToString()
        {
            return $"({DialogType}, {PeerId}) | {Title} {TopMessage}";
        }
    }
}
