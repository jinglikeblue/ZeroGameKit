using System.Collections;
using Example;
using UnityEngine;
using Zero;
using ZeroHot;

namespace PingPong
{
    class PongS2CReceiver : BaseMessageReceiver<Protocols.PongS2C>
    {
        protected override void OnReceive(Protocols.PongS2C m)
        {
            //等待10秒后发送PONG
            ILBridge.Ins.StartCoroutine(this, SendPing());
        }
        
        private IEnumerator SendPing()
        {
            yield return new WaitForSeconds(10);
            Global.Ins.client.Ping();
        }
    }
}
