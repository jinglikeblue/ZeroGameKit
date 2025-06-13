using UnityEngine;

namespace Zero
{
    /// <summary>
    /// ResVerVO扩展方法
    /// </summary>
    public static class ResVerVOExtend
    {
        /// <summary>
        /// 转换为带版本信息的URL格式。返回格式举例："ab/commons.ab?=e788d3ca4f317b88222de7b1064216cf"
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string ToUrlWithVer(this ResVerVO.Item item)
        {
            if (Application.isEditor && false == Runtime.IsHotResEnable)
            {
                return item.name;
            }

            return $"{item.name}?={item.version}";
        }
    }
}