using UnityEngine;

namespace Zero
{
    /// <summary>
    /// Transform工具类
    /// </summary>
    public static class TransformUtility
    {
        /// <summary>
        /// 递归所有子节点，找到指定的GameObject
        /// </summary>
        /// <returns></returns>
        public static Transform DeepFindChild(Transform parent, string childName)
        {
            var child = parent.Find(childName);
            if (child != null)
            {
                return child;
            }

            for (var i = 0; i < parent.childCount; i++)
            {
                child = parent.GetChild(i);
                var tempChild = DeepFindChild(child, childName);
                if (null != tempChild)
                {
                    return tempChild;
                }
            }

            return null;
        }
    }
}