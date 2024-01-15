using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 屏幕工具类
    /// </summary>
    public static class ScreenUtility
    {
        /// <summary>
        /// 切换为竖屏
        /// </summary>
        /// <param name="isPortrait">true表示垂直方向，false表示倒立模式(PortraitUpsideDown)</param>
        /// <param name="isLockOrientation">是否锁定竖屏方向，不随设备翻转而翻转画面</param>
        public static void SwitchToPortrait(bool isPortrait = true, bool isLockOrientation = false)
        {
            Screen.orientation = isPortrait ? ScreenOrientation.Portrait : ScreenOrientation.PortraitUpsideDown;

            if (isLockOrientation)
            {
                SetAutoRotation(false, false, false, false);
            }
            else
            {
                SetAutoRotation(true, true, false, false);
            }

        }

        /// <summary>
        /// 切换为横屏
        /// </summary>
        /// <param name="isLandscapeLeft">true表示默认切换为左横屏模式，false表示右横屏模式(LandscapeRight)</param>
        /// <param name="isLockOrientation">是否锁定竖屏方向，不随设备翻转而翻转画面</param>
        public static void SwitchToLandscape(bool isLandscapeLeft = true, bool isLockOrientation = false)
        {
            Screen.orientation = isLandscapeLeft ? ScreenOrientation.LandscapeLeft : ScreenOrientation.LandscapeRight;

            if (isLockOrientation)
            {
                SetAutoRotation(false, false, false, false);
            }
            else
            {
                SetAutoRotation(false, false, true, true);
            }
        }

        #region 属性

        /// <summary>
        /// 是否是竖屏模式
        /// </summary>
        public static bool IsPortrait
        {
            get
            {
                if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
                {
                    return true;
                }

                if (Screen.orientation == ScreenOrientation.AutoRotation)
                {
                    if (false == Screen.autorotateToLandscapeLeft && false == Screen.autorotateToLandscapeRight)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// 是否是横屏模式
        /// </summary>
        public static bool IsLandscape
        {
            get
            {
                if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
                {
                    return true;
                }

                if (Screen.orientation == ScreenOrientation.AutoRotation)
                {
                    if (false == Screen.autorotateToPortrait && false == Screen.autorotateToPortraitUpsideDown)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        #endregion


        /// <summary>
        /// 自定义设置屏幕旋转
        /// </summary>
        /// <param name="portrait"></param>
        /// <param name="portraitUpsideDown"></param>
        /// <param name="landscapeLeft"></param>
        /// <param name="landscapeRight"></param>
        public static void SetAutoRotation(bool portrait, bool portraitUpsideDown, bool landscapeLeft, bool landscapeRight)
        {
            Screen.orientation = ScreenOrientation.AutoRotation;
            Screen.autorotateToPortrait = portrait;
            Screen.autorotateToPortraitUpsideDown = portraitUpsideDown;
            Screen.autorotateToLandscapeLeft = landscapeLeft;
            Screen.autorotateToLandscapeRight = landscapeRight;
        }
    }
}
