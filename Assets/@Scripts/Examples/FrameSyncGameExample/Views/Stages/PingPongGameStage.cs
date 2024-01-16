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

            UIPanelMgr.Ins.Switch<PingPongGamePanel>();

            //var svc = ResMgr.Ins.Load<ShaderVariantCollection>(AB.EXAMPLES_PING_PONG_MATERIALS.SV_shadervariants_assetPath);
            //svc.WarmUp();
            _game = new PingPongGame(gameObject, OnReceiveBridgeMessage);
            _game.Start();
        }

        void OnReceiveBridgeMessage(object msg)
        {
            var pe = msg as PlayerEntity;
            Debug.Log($"Receive Bridge Message: {pe.speed.x}");
        }
    }
}
