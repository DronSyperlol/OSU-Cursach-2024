using Backend.Tools;

namespace Backend.Controllers.Target.Response
{
    public class TargetStatusResponse : HttpDataBase
    {
        public long peerId { get; set; }
        public bool enable { get; set; }
    }
}
