using UnityEngine;
using ZeroHot;

namespace PingPong
{
    class GameStartNotifyReceiver : BaseMessageReceiver<Protocols.GameStartNotify>
    {
        protected override void OnReceive(Protocols.GameStartNotify m)
        {
            Debug.Log($"GameStartReceiver::OnReceive");
        }
    }
}
