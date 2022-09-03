using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ZeroGameKit;

namespace Knight
{
    public class SettingModule : BaseModule
    {
        public int fps = 60;
        public int quality = 1;
        public int resolution = 1;
        public Vector2Int defaultResolution;
        public Vector2Int resolutionSize;

        public SettingModule()
        {
            defaultResolution.x = Screen.width;
            defaultResolution.y = Screen.height;
            RefreshConfig();
        }

        public override void Dispose()
        {
            
        }

        public void RefreshConfig()
        {
            Application.targetFrameRate = fps;
            QualitySettings.SetQualityLevel(quality);
            switch (resolution)
            {
                case 0:
                    resolutionSize = ScreenUtil.AdaptationResolution(defaultResolution.x, defaultResolution.y, 1280, 720, false);
                    break;
                case 1:
                    resolutionSize = ScreenUtil.AdaptationResolution(defaultResolution.x, defaultResolution.y, 1280, 720, true);
                    break;
                case 2:
                    resolutionSize = defaultResolution;
                    break;
            }
        }

        public void Exit()
        {
            UIWinMgr.Ins.CloseAll();
            UIPanelMgr.Ins.Switch<MenuPanel>();
            G.ResetIns();
        }
    }
}
