using ZeroHot;

namespace PingPong
{
    public class InputRequestReceiver:BaseMessageReceiver<Protocols.InputRequest>
    {
        protected override void OnReceive(Protocols.InputRequest m)
        {
        }
    }
}