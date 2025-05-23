﻿using UnityEngine;
using Zero;

namespace PingPong
{
    class GameStartNotifyReceiver : BaseMessageReceiver<Protocols.GameStartNotify>
    {
        protected override void OnReceive(Protocols.GameStartNotify m)
        {
            Debug.Log($"GameStartReceiver::OnReceive");
            Global.Ins.noticeModule.onHostStart?.Invoke();
        }
    }
}
