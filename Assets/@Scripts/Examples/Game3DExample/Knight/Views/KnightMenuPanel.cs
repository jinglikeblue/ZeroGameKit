﻿using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using Zero;

namespace Knight
{
    public class KnightMenuPanel : AView
    {
        Button _btnEnter;

        protected override void OnInit(object data)
        {
            G.Ins.InitModules();
            _btnEnter = GetChildComponent<Button>("BtnEnter");
        }

        protected override void OnEnable()
        {
            _btnEnter.onClick.AddListener(Enter);

            G.Ins.Audio.Device.StopAll();
            G.Ins.Audio.Device.Play(Assets.Load<AudioClip>(R.MenuBGM_mp3), true);
        }

        protected override void OnDisable()
        {
            _btnEnter.onClick.RemoveListener(Enter);
        }

        private void Enter()
        {
            UIPanelMgr.Ins.Switch<KnightLoadingPanel>(new LoadingVO(typeof(KnightGamePanel)));
        }

    }
}