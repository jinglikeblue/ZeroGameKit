using UnityEngine;

namespace Zero
{
    /// <summary>
    /// Transform的扩展
    /// </summary>
    public static class TransformExtend
    {
        public static void SetLocalX(this Transform t, float value)
        {
            if (t.localPosition.x != value)
            {
                var pos = t.localPosition;
                pos.x = value;
                t.localPosition = pos;
            }
        }

        public static void SetLocalY(this Transform t, float value)
        {
            if (t.localPosition.y != value)
            {
                var pos = t.localPosition;
                pos.y = value;
                t.localPosition = pos;
            }
        }

        public static void SetLocalZ(this Transform t, float value)
        {
            if (t.localPosition.z != value)
            {
                var pos = t.localPosition;
                pos.z = value;
                t.localPosition = pos;
            }
        }

        public static void SetX(this Transform t, float value)
        {
            if (t.position.x != value)
            {
                var pos = t.position;
                pos.x = value;
                t.position = pos;
            }
        }

        public static void SetY(this Transform t, float value)
        {
            if (t.position.y != value)
            {
                var pos = t.position;
                pos.y = value;
                t.position = pos;
            }
        }

        public static void SetZ(this Transform t, float value)
        {
            if (t.position.z != value)
            {
                var pos = t.position;
                pos.z = value;
                t.position = pos;
            }
        }

        /// <summary>
        /// 深度查找子对象
        /// </summary>
        /// <param name="t"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public static Transform DeepFind(this Transform t, string childName)
        {
            return TransformUtility.DeepFindChild(t, childName);
            ;
        }
    }
}