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
    /// RectTransform的扩展
    /// </summary>
    public static class RectTransformExtend
    {
        /// <summary>
        /// RectTrnsform的Stretch布局信息
        /// </summary>
        public struct StretchInfo
        {
            public float left;
            public float right;
            public float top;
            public float bottom;
        }

        /// <summary>
        /// 获取Stretch布局信息
        /// </summary>
        /// <param name="rt"></param>
        /// <returns></returns>
        public static StretchInfo GetStretchInfo(this RectTransform rt)
        {
            StretchInfo info = new StretchInfo();

            var sizeDelta = rt.sizeDelta;
            var anchoredPosition = rt.anchoredPosition;

            info.left = (anchoredPosition.x * 2 - sizeDelta.x) / 2;
            info.right = -info.left - sizeDelta.x;
            info.top = (-anchoredPosition.y * 2 - sizeDelta.y) / 2;
            info.bottom = -info.top - sizeDelta.y;

            return info;
        }

        /// <summary>
        /// 设置Stretch布局信息
        /// PS:仅当某根轴上是Stretch布局的情况下有效
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="info"></param>
        public static void SetStretchInfo(this RectTransform rt, StretchInfo info)
        {
            rt.SetStretchInfo(info.left, info.right, info.top, info.bottom);
        }

        /// <summary>
        /// 设置Stretch布局信息
        /// PS:仅当某根轴上是Stretch布局的情况下有效
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        public static void SetStretchInfo(this RectTransform rt, float left, float right, float top, float bottom)
        {
            var sizeDelta = rt.sizeDelta;
            var localPosition = rt.localPosition;
            var anchoredPosition = rt.anchoredPosition;

            //判断X轴是Stretch布局方式才处理
            if (0 == rt.anchorMin.x && 1 == rt.anchorMax.x)
            {
                anchoredPosition.x = localPosition.x = (left - right) / 2;
                sizeDelta.x = -(left + right);
            }

            //判断Y轴是Stretch布局方式才处理
            if (0 == rt.anchorMin.y && 1 == rt.anchorMax.y)
            {
                anchoredPosition.y = localPosition.y = -(top - bottom) / 2;
                sizeDelta.y = -(top + bottom);
            }

            rt.sizeDelta = sizeDelta;
            rt.localPosition = localPosition;
            rt.anchoredPosition = anchoredPosition;
        }
    }
}
