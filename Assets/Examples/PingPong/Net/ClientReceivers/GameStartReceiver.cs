using ZeroHot;

namespace PingPong
{
    class GameStartReceiver : BaseMessageReceiver<Protocols.GameStartNotify>
    {
        protected override void OnReceive(Protocols.GameStartNotify m)
        {
            
        }
    }
}
