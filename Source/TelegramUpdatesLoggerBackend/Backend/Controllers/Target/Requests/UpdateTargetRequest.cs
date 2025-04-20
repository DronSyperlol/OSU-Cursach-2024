#pragma warning disable IDE1006

using Backend.Tools;

namespace Backend.Controllers.Target.Requests
{
    public class UpdateTargetRequest : HttpDataBase
    {
        public string? phoneNumber { get; set; }
        public long peerId { get; set; }
        public string? accessHash { get; set; }
        public bool? enable { get; set; }
    }
}
