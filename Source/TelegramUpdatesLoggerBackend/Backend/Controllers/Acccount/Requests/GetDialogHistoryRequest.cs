#pragma warning disable IDE1006

using Backend.Tools;
using Core.Types;
using System.ComponentModel.DataAnnotations.Schema;
using TL;

namespace Backend.Controllers.Acccount.Requests
{
    public class GetDialogHistoryRequest : HttpDataBase
    {
        public string? phoneNumber { get; set; }
        public string? dialogType { get; set; }
        public long peerId { get; set; }
        public string? accessHash { get; set; }

        public int offsetId { get; set; } = 0;
        public int limit { get; set; } = 10;

        [NotMapped]
        public InputPeer InputPeer
        {
            get => dialogType switch
            {
                DialogInfo.Types.User => new InputPeerUser(peerId, long.Parse(accessHash ?? throw new ArgumentNullException(nameof(accessHash)))),
                DialogInfo.Types.Chat => new InputPeerChat(peerId),
                DialogInfo.Types.Channel => new InputPeerChannel(peerId, long.Parse(accessHash ?? throw new ArgumentNullException(nameof(accessHash)))),
                _ => throw new ArgumentException("Wrong dialog type")
            };
        }
    }
}
