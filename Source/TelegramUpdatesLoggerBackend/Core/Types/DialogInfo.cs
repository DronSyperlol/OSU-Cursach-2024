namespace Core.Types
{
    public class DialogInfo
    {
        public long PeerId { get; set; }
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
