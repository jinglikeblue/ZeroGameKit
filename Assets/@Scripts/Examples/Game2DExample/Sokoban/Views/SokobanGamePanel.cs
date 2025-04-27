using System;
using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using Zero;

namespace Sokoban
{
    class SokobanGamePanel : AView
    {
        Button _btnBack;
        Button _btnReback;
        Joystick _js;
        SokobanGameStage _stage;

        protected override void OnInit(object data)
        {
            _stage = StageMgr.Ins.Switch<SokobanGameStage>();
            _btnBack = GetChildComponent<Button>("BtnBack");
            _btnReback = GetChildComponent<Button>("BtnReback");
            _js = GetChildComponent<Joystick>("Joystick");            
        }

        protected override void OnDestroy()
        {
            
        }

        protected override void OnEnable()
        {
            _btnBack.onClick.AddListener(OnClickBack);
            _btnReback.onClick.AddListener(OnClickReback);
            _js.onValueChange += OnValueChange;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        private void OnValueChange(Vector2 value)
        {
            EDir newDir;
            if (value == Vector2.zero)
            {
                newDir = EDir.NONE;
            }
            else
            {
                value = value.normalized;
                if (Math.Abs(value.x) > Math.Abs(value.y))
                {
                    //横向移动
                    if (value.x < 0)
                    {
                        newDir = EDir.LEFT;
                    }
                    else
                    {
                        newDir = EDir.RIGHT;
                    }
                }
                else
                {
                    //纵向移动
                    if (value.y < 0)
                    {
                        newDir = EDir.DOWN;
                    }
                    else
                    {
                        newDir = EDir.UP;
                    }
                }
            }

            //Log.CI(Log.COLOR_YELLOW, "移动方向:{0}", newDir);
            _stage.MoveRole(newDir);
        }

        private void OnClickReback()
        {
            _stage.Revoke();
        }

        private void OnClickBack()
        {
            SokobanMsgWin.Show("Exit？", true, () => {
                SokobanGlobal.Ins.Menu.ShowMenu(true);                
            });
        }
    }
}
