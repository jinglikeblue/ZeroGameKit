using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zero;
using ZeroGameKit;
using ZeroHot;

namespace PingPong
{
    [ViewRegister("Assets/@Resources/examples/ping_pong/PingPongGameStage.prefab")]
    public class PingPongGameStage : AView
    {
        PingPongGame _game;

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            
            _game = new PingPongGame(gameObject, OnReceiveBridgeMessage);            
            _game.Start();

            UIPanelMgr.Ins.Switch<PingPongGamePanel>(_game);
        }

        void OnReceiveBridgeMessage(object msg)
        {
            var pe = msg as PlayerEntity;
            Debug.Log($"Receive Bridge Message: {pe.speed.x}");
        }
    }
}
