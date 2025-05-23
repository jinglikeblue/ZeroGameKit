﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using Zero;

namespace Roushan
{
    class StartupPanel : AView
    {
        GameObject btnTap;
        Button btnHelp;


        protected override void OnDisable()
        {
            UIEventListener.Get(btnTap).onClick -= OnClickGo;
            btnHelp.onClick.RemoveListener(OpenHelpWin);
        }

        protected override void OnEnable()
        {
            UIEventListener.Get(btnTap).onClick += OnClickGo;
            btnHelp.onClick.AddListener(OpenHelpWin);
        }

        protected override void OnInit(object data)
        {
            btnTap = GetChild("BtnTap").gameObject;
            btnHelp = GetChildComponent<Button>("BtnHelp");

            ScreenUtility.SwitchToPortrait();            
            Debug.Log($"切换屏幕方向：{Screen.orientation}");
        }

        public void OnClickGo(PointerEventData data)
        {
            Go();
        }

        void Go()
        {            
            StageMgr.Ins.SwitchAsync<GameStage>();            
        }

        void OpenHelpWin()
        {
            UIWinMgr.Ins.OpenAsync<HelpWin>();
        }
    }
}
