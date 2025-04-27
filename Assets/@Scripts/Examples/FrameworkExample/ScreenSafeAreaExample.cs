using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zero;
using Zero;
using ZeroGameKit;
using UnityEngine.UI;
using System.Collections;
using UnityEngine;

namespace Example
{
    static class ScreenSafeAreaExample
    {
        public static void Start()
        {
            UIWinMgr.Ins.Open<ScreenSafeAreaExampleWin>();
        }


    }

    class ScreenSafeAreaExampleWin : WithCloseButtonWin
    {
        public Button btnSafeArea;
        public Button btnRevert;
        public Button btnChangeX;
        public Button btnChangeY;

        RectTransform _uiPanelRT;
        RectTransformExtend.StretchInfo _stretchInfo;

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            _uiPanelRT = UIPanelMgr.Ins.GetComponent<RectTransform>();
            _stretchInfo = _uiPanelRT.GetStretchInfo();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            btnSafeArea.onClick.AddListener(() =>
            {
                Zero.RectTransformUtility.FitScreenSafeArea(_uiPanelRT);
            });

            btnRevert.onClick.AddListener(() =>
            {
                _uiPanelRT.SetStretchInfo(0, 0, 0, 0);
            });

            btnChangeX.onClick.AddListener(() =>
            {
                var si = _uiPanelRT.GetStretchInfo();
                si.left += 10;
                si.right += 10;
                _uiPanelRT.SetStretchInfo(si);
            });

            btnChangeY.onClick.AddListener(() =>
            {
                var si = _uiPanelRT.GetStretchInfo();
                si.top += 10;
                si.bottom += 10;
                _uiPanelRT.SetStretchInfo(si);
            });
        }
    }

}
