using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zero;
using ZeroGameKit;
using Zero;

namespace PingPong
{
    [ViewRegister("Assets/@ab/examples/ping_pong/PingPongGameStage.prefab")]
    public class PingPongGameStage : AView
    {
        PingPongGame _game;

        public PingPongGame Game => _game;

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            
            //调整摄像机为最佳视角
            UpdateCameraDistance();
            
            _game = new PingPongGame(gameObject, OnReceiveBridgeMessage);            
            _game.Start();            
        }

        void OnReceiveBridgeMessage(object msg)
        {
            var pe = msg as PlayerEntity;
            Debug.Log($"Receive Bridge Message: {pe.speed}");
        }

        void UpdateCameraDistance()
        {
            var camera = transform.Find("Objects/Main Camera")?.GetComponent<Camera>();
            if (null == camera)
            {
                Debug.LogError($"找不到摄像机");
            }
            var d = CameraUtility.CalculateBestDistanceToTarget(camera.fieldOfView, 20, 30);
        
            var pos = camera.transform.localPosition;
            pos.y = d;
            camera.transform.localPosition = pos;
        }
    }
}
