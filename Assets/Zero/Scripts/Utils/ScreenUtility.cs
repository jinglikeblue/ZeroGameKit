using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        /// <param name="isPortrait">true表示垂直方向，false表示倒立模式</param>
        /// <param name="isLockOrientation">是否锁定竖屏方向，不随设备翻转而颠倒</param>
        public static void SwitchToPortrait(bool isPortrait, bool isLockOrientation)
        {
            if (isLockOrientation)
            {
                Screen.orientation = isPortrait ? ScreenOrientation.Portrait : ScreenOrientation.PortraitUpsideDown;
            }
            else
            {
                SetAutoRotation(true, true, false, false);
                //TODO 是设备屏幕旋转为isPortrait指定的方向
            }

        }

        /// <summary>
        /// 是否是竖屏模式
        /// </summary>
        public static bool IsPortrait
        {
            get
            {
                if(Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown)
                {
                    return true;
                }

                if(Screen.orientation == ScreenOrientation.AutoRotation)
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
        /// 切换为横屏
        /// </summary>
        public static void SwitchToLandscape()
        {
            SetAutoRotation(false, false, true, true);
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

        /// <summary>
        /// 横竖屏自由旋转
        /// </summary>
        public static void SwitchToPortraitAndLandscape()
        {
            SetAutoRotation(true, true, true, true);
        }

        /// <summary>
        /// 是否可以横竖屏自由切换
        /// </summary>
        /// <returns></returns>
        public static bool IsPortraitAndLandscape()
        {
            if(Screen.orientation != ScreenOrientation.AutoRotation)
            {
                return false;
            }

            if (false == Screen.autorotateToPortrait && false == Screen.autorotateToPortraitUpsideDown)
            {
                //不能旋转为竖屏
                return false;
            }

            if (false == Screen.autorotateToLandscapeLeft && false == Screen.autorotateToLandscapeRight)
            {
                //不能旋转为横屏
                return false;
            }

            return true;
        }

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
