using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using ZeroHot;

namespace PingPong
{
    //[ViewRegister("examples/ping_pong/PingPongMenuPanel.prefab")]
    public class PingPongMenuPanel : AView
    {
        Button btnPVE;
        Button btnPVP;
        Button btnExit;

        protected override void OnInit(object data)
        {
            base.OnInit(data);

            ScreenUtility.SwitchToPortrait();            
            Debug.Log($"切换屏幕方向：{Screen.orientation}");       
        }        

        protected override void OnEnable()
        {
            base.OnEnable();

            btnExit.onClick.AddListener(Exit);
            btnPVE.onClick.AddListener(EnterPVE);
            btnPVP.onClick.AddListener(EnterPVP);
        }



        protected override void OnDisable()
        {
            base.OnDisable();

            btnExit.onClick.RemoveListener(Exit);
            btnPVE.onClick.RemoveListener(EnterPVE);
            btnPVP.onClick.RemoveListener(EnterPVP);
        }

        private void EnterPVP()
        {
            UIWinMgr.Ins.Open<PingPongNetBattleMenuWin>();
        }

        private void EnterPVE()
        {
            UIPanelMgr.Ins.Switch<PingPongGamePanel>();            
        }

        private void Exit()
        {
            UIPanelMgr.Ins.Switch<MenuPanel>();
            ScreenUtility.SwitchToLandscape();
            Debug.Log($"切换屏幕方向：{Screen.orientation}");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }


    }
}
