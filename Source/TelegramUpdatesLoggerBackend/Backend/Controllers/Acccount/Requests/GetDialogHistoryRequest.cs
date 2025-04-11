#pragma warning disable IDE1006

namespace Backend.Controllers.Acccount.Requests
{
    public class GetDialogHistoryRequest
    {
        public string dialogType { get; set; }
        public long peerId { get; set; }
        public long? accessHash { get; set; }

        public int offsetId { get; set; } = 0;
        public int limit { get; set; } = 10;
    }
}
