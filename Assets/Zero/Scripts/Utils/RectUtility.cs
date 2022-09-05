using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 矩形实用类
    /// </summary>
    public class RectUtility
    {
        /// <summary>
        /// 判断矩形a是否完整覆盖矩形b，矩形b的四个角都在矩形a的区域内
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Contains(Rect a, Rect b)
        {
            if (a.xMin <= b.xMin
                && a.yMin <= b.yMin
                && a.xMax >= b.xMax
                && a.yMax >= b.yMax)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 根据目标宽高来适配源数据宽高，使新的宽高保证和源宽高比例一致的情况下，与目标宽高匹配（至少一条边相同）
        /// </summary>
        /// <param name="sourceSize">源宽高</param>
        /// <param name="targetSize">目标宽高</param>
        /// <param name="isUpper">true：适配后宽高总是大于等于期望值   false：适配后宽高总是小于等于期望值</param>
        /// <returns></returns>
        public static Vector2 AdaptSize(Vector2 sourceSize, Vector2 targetSize, bool isUpper)
        {
            Vector2 outputSize = AdaptSize(sourceSize.x, sourceSize.y, targetSize.x, targetSize.y, isUpper);
            return outputSize;
        }

        /// <summary>
        /// 根据目标宽高来适配源数据宽高，使新的宽高保证和源宽高比例一致的情况下，与目标宽高匹配（至少一条边相同）
        /// </summary>
        /// <param name="sourceSize">源宽高</param>
        /// <param name="targetSize">目标宽高</param>
        /// <param name="isUpper">true：适配后宽高总是大于等于期望值   false：适配后宽高总是小于等于期望值</param>
        /// <returns></returns>
        public static Vector2Int AdaptSize(Vector2Int sourceSize, Vector2Int targetSize, bool isUpper)
        {
            var size = AdaptSize(sourceSize.x, sourceSize.y, targetSize.x, targetSize.y, isUpper);
            Vector2Int outputSize = new Vector2Int((int)size.x, (int)size.y);
            return outputSize;
        }

        /// <summary>
        /// 根据目标宽高来适配源数据宽高，使新的宽高保证和源宽高比例一致的情况下，与目标宽高匹配（至少一条边相同）
        /// </summary>
        /// <param name="sW">原始宽度</param>
        /// <param name="sH">原始高度</param>
        /// <param name="tW">适配宽度（期望）</param>
        /// <param name="tH">适配高度（期望）</param>
        /// <param name="isUpper">true：适配后宽高总是大于等于期望值   false：适配后宽高总是小于等于期望值</param>
        /// <returns>适配后的宽高</returns>
        public static Vector2 AdaptSize(float sW, float sH, float tW, float tH, bool isUpper)
        {
            Vector2 fit = new Vector2();

            //当前宽高比
            float sK = sW / sH;
            //目标宽高比
            float tK = tW / tH;

            //0:直接使用适配宽高   1：使用适配宽度矫正分辨率   2：使用适配高度矫正分辨率
            int fixType = 0;

            if (sK > tK)
            {
                if (isUpper)
                {
                    fixType = 2;
                }
                else
                {
                    fixType = 1;
                }
            }
            else if (sK < tK)
            {
                if (isUpper)
                {
                    fixType = 1;
                }
                else
                {
                    fixType = 2;
                }
            }

            switch (fixType)
            {
                case 0:
                    fit.x = tW;
                    fit.y = tH;
                    break;
                case 1:
                    //以目标宽度矫正宽高
                    fit.x = tW;
                    fit.y = tW / sK;
                    break;
                case 2:
                    //以目标高度矫正宽高
                    fit.x = tH * sK;
                    fit.y = tH;
                    break;
            }

            return fit;
        }


    }
}
