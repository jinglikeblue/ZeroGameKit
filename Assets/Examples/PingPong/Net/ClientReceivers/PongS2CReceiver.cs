using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroHot;

namespace PingPong
{
    class PongS2CReceiver : BaseMessageReceiver<Protocols.PongS2C>
    {
        protected override void OnReceive(Protocols.PongS2C m)
        {
            
        }
    }
}
