using ZeroHot;

namespace PingPong
{
    public class GameReadyRequestReceiver:BaseMessageReceiver<Protocols.GameReadyRequest>
    {
        protected override void OnReceive(Protocols.GameReadyRequest m)
        {
            
        }
    }
}