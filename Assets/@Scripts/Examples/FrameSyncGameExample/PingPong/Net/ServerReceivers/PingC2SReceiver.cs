using System.Collections;
using Example;
using UnityEngine;
using Zero;
using ZeroHot;

namespace PingPong
{
    class PingC2SReceiver:BaseMessageReceiver<Protocols.PingC2S>
    {
        private Protocols.PingC2S _pingBody;
        
        protected override void OnReceive(Protocols.PingC2S m)
        {
            _pingBody = m;
            //等待10秒后发送PONG
            ILBridge.Ins.StartCoroutine(this, SendPong());
        }

        private IEnumerator SendPong()
        {
            yield return new WaitForSeconds(10);
            Global.Ins.host.Pong(_pingBody);
        }
    }
}