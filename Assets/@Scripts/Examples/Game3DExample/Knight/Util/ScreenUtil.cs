using UnityEngine;
using Zero;

namespace Knight
{
    public class ScreenUtil
    {
        /// <summary>
        /// 适配分辨率，在保持原始宽高比的情况下，尽量使分辨率与期望分辨率匹配（至少一条边相同）
        /// </summary>
        /// <param name="sW">原始宽度</param>
        /// <param name="sH">原始高度</param>
        /// <param name="tW">适配宽度（期望）</param>
        /// <param name="tH">适配高度（期望）</param>
        /// <param name="isUpper">true：适配后宽高总是大于等于期望值   false：适配后宽高总是小于等于期望值</param>
        /// <returns></returns>
        public static Vector2Int AdaptationResolution(int sW, int sH, int tW, int tH, bool isUpper)
        {
            var size = RectUtility.AdaptSize(sW, sH, tW, tH, isUpper);            
            return new Vector2Int((int)size.x, (int)size.y);
        }
    }
}