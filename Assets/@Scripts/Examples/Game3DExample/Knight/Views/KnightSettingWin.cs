using Knight;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;

namespace Knight
{
    public class KnightSettingWin : WithCloseButtonWin
    {
        Toggle _tLowResolution;
        Toggle _tNormalResolution;
        Toggle _tHighResolution;

        Toggle _t30fps;
        Toggle _t60fps;

        Toggle _tLowQuality;
        Toggle _tMiddleQuality;
        Toggle _tHighQuality;

        public Button btnExit;

        protected override void OnInit(object data)
        {
            base.OnInit(data);

            _tLowResolution = GetChildComponent<Toggle>("ResolutionSet/List/TLow");
            _tNormalResolution = GetChildComponent<Toggle>("ResolutionSet/List/TNormal");
            _tHighResolution = GetChildComponent<Toggle>("ResolutionSet/List/THigh");

            _t30fps = GetChildComponent<Toggle>("FPSSet/List/T30");
            _t60fps = GetChildComponent<Toggle>("FPSSet/List/T60");

            _tLowQuality = GetChildComponent<Toggle>("QualitySet/List/TLow");
            _tMiddleQuality = GetChildComponent<Toggle>("QualitySet/List/TMiddle");
            _tHighQuality = GetChildComponent<Toggle>("QualitySet/List/THigh");
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            PointerClickEventListener.Get(_tLowResolution.gameObject).onEvent += SetResolution;
            PointerClickEventListener.Get(_tNormalResolution.gameObject).onEvent += SetResolution;
            PointerClickEventListener.Get(_tHighResolution.gameObject).onEvent += SetResolution;

            PointerClickEventListener.Get(_t30fps.gameObject).onEvent += UpdateSetting;
            PointerClickEventListener.Get(_t60fps.gameObject).onEvent += UpdateSetting;

            PointerClickEventListener.Get(_tLowQuality.gameObject).onEvent += UpdateSetting;
            PointerClickEventListener.Get(_tMiddleQuality.gameObject).onEvent += UpdateSetting;
            PointerClickEventListener.Get(_tHighQuality.gameObject).onEvent += UpdateSetting;

            btnExit.onClick.AddListener(Exit);

            SyncUI();
            UpdateSetting(null);                        
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            PointerClickEventListener.Get(_tLowResolution.gameObject).onEvent -= SetResolution;
            PointerClickEventListener.Get(_tNormalResolution.gameObject).onEvent -= SetResolution;
            PointerClickEventListener.Get(_tHighResolution.gameObject).onEvent -= SetResolution;

            PointerClickEventListener.Get(_t30fps.gameObject).onEvent -= UpdateSetting;
            PointerClickEventListener.Get(_t60fps.gameObject).onEvent -= UpdateSetting;

            PointerClickEventListener.Get(_tLowQuality.gameObject).onEvent -= UpdateSetting;
            PointerClickEventListener.Get(_tMiddleQuality.gameObject).onEvent -= UpdateSetting;
            PointerClickEventListener.Get(_tHighQuality.gameObject).onEvent -= UpdateSetting;

            btnExit.onClick.RemoveListener(Exit);
        }

        private void Exit()
        {
            G.Ins.Setting.Exit();
        }

        private void SetResolution(PointerEventData obj)
        {
            UpdateSetting(obj);
            if (Debug.isDebugBuild)
            {
                GUILog.Clear();
                GUILog.Show(string.Format("{0} 分辨率设置为：{1}", DateTime.Now.ToString("HH:mm:ss"), G.Ins.Setting.resolutionSize));
            }
        }

        void UpdateSetting(PointerEventData obj)
        {
            G.Ins.Setting.fps = _t30fps.isOn ? 30 : 60;

            if (_tLowQuality.isOn)
            {
                G.Ins.Setting.quality = 0;
            }
            else if (_tMiddleQuality.isOn)
            {
                G.Ins.Setting.quality = 1;
            }
            else if (_tHighQuality.isOn)
            {
                G.Ins.Setting.quality = 2;
            }

            if (_tLowResolution.isOn)
            {
                G.Ins.Setting.resolution = 0;
                G.Ins.Setting.resolutionSize = ScreenUtil.AdaptationResolution(G.Ins.Setting.defaultResolution.x, G.Ins.Setting.defaultResolution.y, 1280, 720, false);
            }
            else if (_tNormalResolution.isOn)
            {
                G.Ins.Setting.resolution = 1;
                
            }
            else if (_tHighResolution.isOn)
            {
                G.Ins.Setting.resolution = 2;
                
            }

            G.Ins.Setting.RefreshConfig();
        }

        void SyncUI()
        {
            if (G.Ins.Setting.fps == 60)
            {
                _t60fps.isOn = true;
            }
            else
            {
                _t30fps.isOn = true;
            }

            switch (G.Ins.Setting.quality)
            {
                case 0:
                    _tLowQuality.isOn = true;
                    break;
                case 1:
                    _tMiddleQuality.isOn = true;
                    break;
                case 2:
                    _tHighQuality.isOn = true;
                    break;
            }

            switch (G.Ins.Setting.resolution)
            {
                case 0:
                    _tLowResolution.isOn = true;
                    break;
                case 1:
                    _tNormalResolution.isOn = true;
                    break;
                case 2:
                    _tHighResolution.isOn = true;
                    break;
            }            
        }
    }
}