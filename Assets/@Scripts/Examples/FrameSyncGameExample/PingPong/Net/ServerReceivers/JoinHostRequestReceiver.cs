using UnityEngine;
using ZeroHot;

namespace PingPong
{
    public class JoinHostRequestReceiver:BaseMessageReceiver<Protocols.JoinHostRequest>
    {
        protected override void OnReceive(Protocols.JoinHostRequest m)
        {
            var host = Global.Ins.netModule.host;
            //TODO 通知游戏开始
            host.GameStart();
        }
    }
}