using TL;

namespace Core.Services.Types
{
    class WatchingTarget
    {
        public WatchingTarget(long dbId, InputPeer inputPeer)
        {
            TargetDbId = dbId;
            InputPeer = inputPeer;
        }
        public long TargetDbId { get; }
        public InputPeer InputPeer { get; }

    }
}
