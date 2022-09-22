using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Zero
{
    /// <summary>
    /// 矩形区域工具
    /// </summary>
    public class RectTransformUtility
    {
        /// <summary>
        /// 适配渲染安全区域
        /// </summary>
        /// <param name="rt">调整的RectTransform</param>
        /// <param name="safeAreaRect">安全区域的矩形数据(该矩形表示的区域是以左下角为原点)</param>
        public static void FitRenderSafeArea(RectTransform rt, Rect safeAreaRect)
        {
            var cs = rt.GetComponentInParent<CanvasScaler>();
            var canvasRT = cs.GetComponent<RectTransform>();

            var deviceRenderScale = new Vector2(Screen.width / canvasRT.rect.width, Screen.height / canvasRT.rect.height);

            var sizeDelta = rt.sizeDelta;
            var localPosition = rt.localPosition;
            var anchoredPosition = rt.anchoredPosition;

            if(0 == rt.anchorMin.x && 1 == rt.anchorMax.x)
            {

                //X轴上的stretch
                
                var stretchLeft = safeAreaRect.xMin;                
                var stretchRight = Screen.width - safeAreaRect.xMax;

                stretchLeft /= deviceRenderScale.x;
                stretchRight /= deviceRenderScale.x;

                anchoredPosition.x = localPosition.x = (stretchLeft - stretchRight) / 2;
                sizeDelta.x = -(stretchLeft + stretchRight);
            }
            
            if(0 == rt.anchorMin.y && 1 == rt.anchorMax.y)
            {
                //Y轴上的stretch，safeAreaRect中Y轴的原点在屏幕下方，向上依次增大

                var stretchTop = Screen.height - safeAreaRect.yMax;
                var stretchBottom = safeAreaRect.yMin; 

                stretchTop /= deviceRenderScale.y;
                stretchBottom /= deviceRenderScale.y;

                anchoredPosition.y = localPosition.y = -(stretchTop - stretchBottom) / 2;
                sizeDelta.y = -(stretchTop + stretchBottom);
            }

            rt.sizeDelta = sizeDelta;
            rt.localPosition = localPosition;
            rt.anchoredPosition = anchoredPosition;
        }
    }
}
