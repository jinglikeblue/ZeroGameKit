using UnityEngine;
using Zero;

namespace PingPong
{
    public class JoinHostRequestReceiver:BaseMessageReceiver<Protocols.JoinHostRequest>
    {
        protected override void OnReceive(Protocols.JoinHostRequest m)
        {
            var host = Global.Ins.netModule.host;
            //通知游戏开始
            host.GameStart();
            Global.Ins.noticeModule.onHostStart?.Invoke();
        }
    }
}