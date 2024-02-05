using System.Collections;
using Example;
using Jing;
using UnityEngine;
using Zero;
using ZeroHot;

namespace PingPong
{
    class PingC2SReceiver:BaseMessageReceiver<Protocols.PingC2S>
    {
        protected override void OnReceive(Protocols.PingC2S m)
        {
            Global.Ins.netModule.lastReceivePingPongUTC = TimeUtility.NowUtcMilliseconds;
            
            Global.Ins.netModule.host.Pong(m);
        }
    }
}