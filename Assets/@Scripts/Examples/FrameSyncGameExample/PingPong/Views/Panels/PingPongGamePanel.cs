using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using Zero;

namespace PingPong
{
    public class PingPongGamePanel : AView
    {
        PingPongGame _game;
        RectTransform inputCatcher;

        Button btnSetting;

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            var stage = StageMgr.Ins.Switch<PingPongGameStage>();
            _game = stage.Game;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            StartCoroutine(RefreshInfo());

            inputCatcher.GetComponent<PointerDragEventListener>().onEvent += OnPointerDrag;

            PointerDownEventListener.Get(inputCatcher.gameObject).onEvent += OnPointerDrag;

            PointerUpEventListener.Get(inputCatcher.gameObject).onEvent += (e) =>
            {
                _game.SetMoveCoefficient(0);
            };

            btnSetting.onClick.AddListener(OpenSettingWin);

            StartCoroutine(GameEndCheck());
        }

        IEnumerator GameEndCheck()
        {
            WaitForSeconds halfSeconds = new WaitForSeconds(0.5f);
            while (true)
            {
                var value = _game.tempStorage.Get(Define.WINNER_STORAGE_KEY);
                if (null == value)
                {
                    yield return halfSeconds;
                    continue;
                }
                int winner = (int)value;
                ShowControllerWin("结束", winner == 0 ? "你赢了" : "你输了", false);
                break;
            }
        }

        void ShowControllerWin(string title, string content, bool isShowContinueButton = true)
        {
            var win = UIWinMgr.Ins.Open<PingPongGameControlWin>();
            win.btnContinue.gameObject.SetActive(isShowContinueButton);
            win.SetText(title, content);
            win.onContinueSelected += () =>
            {
                _game.Continue();
            };
            win.onRestartSelected += () =>
            {
                _game.Destroy();
                var stage = StageMgr.Ins.Switch<PingPongGameStage>();
                _game = stage.Game;
                StopAllCoroutines();
                StartCoroutine(RefreshInfo());
                StartCoroutine(GameEndCheck());
            };
            win.onExitSelected += () =>
            {
                UIPanelMgr.Ins.Switch<PingPongMenuPanel>();
            };
        }

        private void OpenSettingWin()
        {
            _game.Pause();
            ShowControllerWin("设置", "游戏已暂停");
        }

        private void OnPointerDrag(PointerEventData obj)
        {
            float moveCoefficient = 0;
            var camera = CameraMgr.Ins.GetUICamera();
            Vector2 localPos;
            UnityEngine.RectTransformUtility.ScreenPointToLocalPointInRectangle(inputCatcher, obj.position, camera, out localPos);
            GUIDebugInfo.SetInfo("[Drag] Mouse Position:", $"{obj.position}_{localPos}");

            moveCoefficient = localPos.x / (inputCatcher.rect.width / 2);
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
