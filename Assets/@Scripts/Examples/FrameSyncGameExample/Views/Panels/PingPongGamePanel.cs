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
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            StartCoroutine(RefreshInfo());
        }

        IEnumerator RefreshInfo()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);

                if (_game.gameCore.FrameData == null)
                {
                    continue;
                }
                textFrames.text = $"Frames: {_game.gameCore.FrameData.elapsedFrames}";                
            }
        }
    }
}
