using ZeroHot;

namespace PingPong
{
    public class JoinHostRequestReceiver:BaseMessageReceiver<Protocols.JoinHostRequest>
    {
        protected override void OnReceive(Protocols.JoinHostRequest m)
        {
            
        }
    }
}