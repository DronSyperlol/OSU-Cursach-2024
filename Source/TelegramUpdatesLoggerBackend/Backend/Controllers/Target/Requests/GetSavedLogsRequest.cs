#pragma warning disable IDE1006

using Backend.Tools;

namespace Backend.Controllers.Target.Requests
{
    public class GetSavedLogsRequest : HttpDataBase
    {
        public string? phoneNumber { get; set; }
        public long peerId { get; set; }
        public int offsetId { get; set; } = 0;
        public int limit { get; set; } = 10;
    }
}
