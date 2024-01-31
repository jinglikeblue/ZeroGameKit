using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroHot;

namespace PingPong
{
    class FrameInputNotifyReceiver : BaseMessageReceiver<Protocols.FrameInputNotify>
    {
        protected override void OnReceive(Protocols.FrameInputNotify m)
        {
            
        }
    }
}
