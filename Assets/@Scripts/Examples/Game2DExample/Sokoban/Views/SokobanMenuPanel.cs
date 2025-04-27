using System;
using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using Zero;

namespace Sokoban
{
    public class SokobanMenuPanel : AView
    {        
        Button _btnStart;

        Button _btnSelectLevel;

        Button _btnCredits;

        public Button btnExit;

        protected override void OnInit(object data)
        {
            //生成单例
            SokobanGlobal.Ins.InitModules();

            _btnStart = GetChildComponent<Button>("StartupMenu/BtnStart");
            _btnSelectLevel = GetChildComponent<Button>("StartupMenu/BtnSelectLevel");
            _btnCredits = GetChildComponent<Button>("StartupMenu/BtnCredits");
            StageMgr.Ins.Clear();
        }

        protected override void OnEnable()
        {
            _btnStart.onClick.AddListener(Start);
            _btnSelectLevel.onClick.AddListener(SelectLevel);
            _btnCredits.onClick.AddListener(ShowCredits);

            btnExit.onClick.AddListener(Exit);
        }

        protected override void OnDisable()
        {
            _btnStart.onClick.RemoveListener(Start);
            _btnSelectLevel.onClick.RemoveListener(SelectLevel);
            _btnCredits.onClick.RemoveListener(ShowCredits);

            btnExit.onClick.RemoveListener(Exit);
        }

        private void Exit()
        {
            SokobanGlobal.ResetIns();
            UIPanelMgr.Ins.Switch<MenuPanel>();
        }

        private void Start()
        {
            SokobanGlobal.Ins.Level.EnterLevel(1);                   
        }

        private void SelectLevel()
        {
            UIWinMgr.Ins.Open<LevelSelectWin>();
        }

        private void ShowCredits()
        {
            UIWinMgr.Ins.Open<CreditsWin>();
        }
    }
}