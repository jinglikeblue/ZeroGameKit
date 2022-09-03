using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using ZeroHot;

namespace Sokoban
{
    class ScreenChecker
    {
        int lastSW = 0;
        int lastSH = 0;
        public ScreenChecker()
        {
            lastSW = Screen.width;
            lastSH = Screen.height;
            ILBridge.Ins.StartCoroutine(ScreenCheckLoop());
        }

        IEnumerator ScreenCheckLoop()
        {
            do
            {
                if(lastSW != Screen.width || lastSH != Screen.height)
                {
                    lastSW = Screen.width;
                    lastSH = Screen.height;
                    Debug.LogFormat("屏幕分辨率改变：{0} x {1}", lastSW, lastSH);
                    SokobanGlobal.Ins.Notice.onScreenSizeChange?.Invoke();
                }
                yield return new WaitForSeconds(0.1f);
            } while (true);
        }        
    }
}
