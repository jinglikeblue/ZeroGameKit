using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using ZeroHot;

namespace PingPong
{
    public class PingPongGamePanel : AView
    {
        PingPongGame _game;

        RectTransform inputCatcher;

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            _game = data as PingPongGame;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            StartCoroutine(RefreshInfo());
            
            inputCatcher.GetComponent<PointerDragEventListener>().onEvent += OnPointerDrag;
        }

        private void OnPointerDrag(PointerEventData obj)
        {
            var camera = CameraMgr.Ins.GetUICamera();
            Vector2 localPos;
            UnityEngine.RectTransformUtility.ScreenPointToLocalPointInRectangle(inputCatcher, obj.position, camera, out localPos);
            GUIDebugInfo.SetInfo("[Drag] Mouse Position:", $"{obj.position}_{localPos}");

            var moveCoefficient = localPos.x / (inputCatcher.rect.width / 2);
            moveCoefficient = Mathf.Clamp(moveCoefficient, -1, 1);
            GUIDebugInfo.SetInfo("[Drag] Move Coefficient:", $"{moveCoefficient}");

            _game.SetMoveCoefficient(moveCoefficient);
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
                if (tn.TotalSeconds < 1)
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
