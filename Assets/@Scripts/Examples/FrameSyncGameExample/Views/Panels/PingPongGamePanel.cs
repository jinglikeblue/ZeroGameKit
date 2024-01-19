using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using ZeroHot;

namespace PingPong
{
    public class PingPongGamePanel : AView
    {
        PingPongGame _game;

        Text textFrames;

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            _game = data as PingPongGame;
            textFrames.text = "";
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            StartCoroutine(RefreshInfo());
        }

        IEnumerator RefreshInfo()
        {
            ulong lastFrame = 0;
            var time = DateTime.Now;
            while (true)
            {
                if (_game.gameCore.FrameData == null)
                {
                    yield return null;
                    continue;
                }

                var now = DateTime.Now;
                var tn = now - time;
                if(tn.TotalSeconds < 1)
                {
                    yield return null;
                    continue;
                }

                time = now;

                var diffFrame = _game.gameCore.FrameData.elapsedFrames - lastFrame;
                lastFrame = _game.gameCore.FrameData.elapsedFrames;
                Zero.GUIDebugInfo.SetInfo("Frames", $"{lastFrame}(+{diffFrame}) TimeInterval:{tn.TotalSeconds}");

                //textFrames.text = $"Frames: {_game.gameCore.FrameData.elapsedFrames}";                
            }
        }
    }
}
