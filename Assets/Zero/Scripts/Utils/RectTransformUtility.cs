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
        /// 适配屏幕渲染安全区域（针对异形屏）
        /// </summary>
        /// <param name="rectTransform">调整的RectTransform</param>
        public static void FitScreenSafeArea(RectTransform rectTransform)
        {
            var safeAreaRect = Screen.safeArea;           

            float left = safeAreaRect.xMin;
            float right = Screen.width - safeAreaRect.xMax;

            //Y轴上的stretch，safeAreaRect中Y轴的原点在屏幕下方，向上依次增大
            float top = Screen.height - safeAreaRect.yMax;
            float bottom = safeAreaRect.yMin;

            
            //如果已经在界面上，则根据渲染缩放数据更新Stretch值
            var cs = rectTransform.GetComponentInParent<CanvasScaler>();
            if (cs != null)
            {
                var canvasRT = cs.GetComponent<RectTransform>();
                var deviceRenderScale = new Vector2(Screen.width / canvasRT.rect.width, Screen.height / canvasRT.rect.height);

                left /= deviceRenderScale.x;
                right /= deviceRenderScale.x;

                top /= deviceRenderScale.y;
                bottom /= deviceRenderScale.y;
            }

            rectTransform.SetStretchInfo(left, right, top, bottom);
        }
    }
}
