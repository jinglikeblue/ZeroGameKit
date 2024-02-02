using ZeroHot;

namespace PingPong
{
    class PingC2SReceiver:BaseMessageReceiver<Protocols.PongS2C>
    {
        protected override void OnReceive(Protocols.PongS2C m)
        {
            
        }
    }
}