using Config;
using TL;

namespace Core.Types
{
    public class DialogInfo
    {
        public const int MAX_MESSAGE_PREVIEW = 50;

        public DialogInfo(User reference, string photoFilename, string topMessage, int topMessageId)
        {
            PeerId = reference.id;
            AccessHash = reference.access_hash.ToString();
            Title = reference.first_name + " " + reference.last_name;
            TopMessage = topMessage.Length > MAX_MESSAGE_PREVIEW ? topMessage[..(MAX_MESSAGE_PREVIEW - 3)] + "..." : topMessage;
            TopMessageId = topMessageId;
            DialogType = reference.IsBot ? Types.Bot : Types.User;
            _isBot = reference.IsBot;
            PhotoUrl = ProgramConfig.Path.Static + photoFilename;
            _inputPeer = reference.ToInputPeer();
        }
        public DialogInfo(Chat reference, string photoFilename, string topMessage, int topMessageId)
        {
            PeerId = reference.id;
            AccessHash = null;
            Title = reference.Title;
            TopMessage = topMessage.Length > MAX_MESSAGE_PREVIEW ? topMessage[..(MAX_MESSAGE_PREVIEW - 3)] + "..." : topMessage;
            TopMessageId = topMessageId;
            DialogType = Types.Chat;
            PhotoUrl = ProgramConfig.Path.Static + photoFilename;
            _inputPeer = reference.ToInputPeer();
        }
        public DialogInfo(Channel reference, string photoFilename, string topMessage, int topMessageId)
        {
            PeerId = reference.id;
            AccessHash = reference.access_hash.ToString();
            Title = reference.Title;
            TopMessage = topMessage.Length > MAX_MESSAGE_PREVIEW ? topMessage[..(MAX_MESSAGE_PREVIEW - 3)] + "..." : topMessage;
            TopMessageId = topMessageId;
            DialogType = Types.Channel;
            PhotoUrl = ProgramConfig.Path.Static + photoFilename;
            _inputPeer = reference.ToInputPeer();
            _isChannel = true;
        }

        public long     PeerId { get; set; }
        public string?  AccessHash { get; set; }
        public string   Title { get; set; }
        public string   TopMessage { get; set; }
        public int      TopMessageId { get; set; }
        public bool     IsTarget { get; set; }
        public string   DialogType { get; set; }
        public string   PhotoUrl { get; set; }



        private bool _isBot = false;
        private bool _isChannel = false;
        private InputPeer? _inputPeer = null;

        public struct Types
        {
            public const string User = "User";
            public const string Chat = "Chat";
            public const string Channel = "Channel";
            public const string Bot = "User|Bot";
        }

        public bool IsBot() => _isBot;
        public bool IsChannel() => _isChannel;
        public InputPeer? GetInputPeer() => _inputPeer;

        public override string ToString()
        {
            return $"({DialogType}, {PeerId}) | {Title} {TopMessage}";
        }
    }
}
