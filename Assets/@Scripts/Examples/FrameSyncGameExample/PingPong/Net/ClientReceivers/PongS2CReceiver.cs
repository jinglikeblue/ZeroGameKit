using System.Collections;
using Example;
using Jing;
using UnityEngine;
using Zero;
using ZeroHot;

namespace PingPong
{
    class PongS2CReceiver : BaseMessageReceiver<Protocols.PongS2C>
    {
        protected override void OnReceive(Protocols.PongS2C m)
        {
            Global.Ins.netModule.heartbeat.PongReceived();
            
            NetDelayInfo info = new NetDelayInfo();
            info.c2s = (int)(m.serverUTC - m.clientUTC);
            info.s2c = (int)(TimeUtility.NowUtcMilliseconds - m.serverUTC);
            info.total = info.c2s + info.s2c;
        }
    }
}
