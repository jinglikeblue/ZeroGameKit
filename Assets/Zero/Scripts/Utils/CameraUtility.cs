using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 相机工具类
    /// </summary>
    public static class CameraUtility
    {
        /// <summary>
        /// 在视野确定的情况下，计算要完整看到目标的最佳距离。
        /// </summary>
        /// <param name="fieldOfView">竖向视野值</param>
        /// <param name="targetW">目标的宽度</param>
        /// <param name="targetH">目标的高度</param>
        /// <returns>返回的是相机正目标中心点时，距离目标最佳的观测距离</returns>
        public static float CalculateBestDistanceToTarget(float fieldOfView, float targetW, float targetH)
        {
            var aspectRatio = Screen.width / (float)Screen.height;
            var panelRatio = targetW / targetH;

            var useVFOV = true;
            float targetHalfSize = targetH / 2;
            if (aspectRatio < panelRatio)
            {
                useVFOV = false;
                targetHalfSize = targetW / 2;
            }

            var fov = useVFOV ? fieldOfView : Camera.VerticalToHorizontalFieldOfView(fieldOfView, aspectRatio);

            var distance = (1 / Mathf.Tan((fov / 2) * Mathf.Deg2Rad)) * targetHalfSize;

            return distance;
        }
    }
}
