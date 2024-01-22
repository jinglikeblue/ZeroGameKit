using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Jing.FixedPointNumber
{
    /// <summary>
    /// 几何运算
    /// </summary>
    public static class Geometry
    {
        /// <summary>
        /// 检查两个矩形是否相交
        /// </summary>
        /// <returns></returns>
        public static bool CheckIntersect(Rect a, Rect b)
        {
            if (a.left > b.right || b.left > a.right)
            {
                return false; // 两个矩形的水平边没有交集
            }

            if (a.top < b.bottom || b.top < a.bottom)
            {
                return false; // 两个矩形的垂直边没有交集
            }

            return true; // 两个矩形相交
        }

        /// <summary>
        /// 检查圆形和矩形是否相交
        /// </summary>
        /// <param name="a">圆形</param>
        /// <param name="b">矩形</param>
        /// <returns>如果圆形和矩形相交，则返回true；否则返回false</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckIntersect(Round a, Rect b)
        {
            // 计算最接近圆形的点的X坐标，将其限定在矩形的左右边界之间
            var closestX = Math.Clamp(a.x, b.left, b.right);

            // 计算最接近圆形的点的Y坐标，将其限定在矩形的上下边界之间
            var closestY = Math.Clamp(a.y, b.top, b.bottom);

            // 计算圆心到最接近点的X距离
            var distanceX = a.x - closestX;

            // 计算圆心到最接近点的Y距离
            var distanceY = a.y - closestY;

            // 计算圆心到最接近点的距离的平方
            var distanceSquared = (distanceX * distanceX) + (distanceY * distanceY);

            // 如果距离的平方小于等于圆形半径的平方，则表示圆形和矩形相交
            return distanceSquared <= (a.radius * a.radius);
        }

        /// <summary>
        /// 检查圆形和矩形是否相交
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool CheckIntersect(Rect b, Round a)
        {
            return CheckIntersect(a, b);
        }

        /// <summary>
        /// 检查线段和圆形是否相交
        /// </summary>
        /// <param name="ls"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static bool CheckIntersect(LineSegment ls, Round r)
        {
            throw new Exception($"暂未实现");
        }

        /// <summary>
        /// 检查线段和圆形是否相交
        /// </summary>
        /// <param name="r"></param>
        /// <param name="ls"></param>
        /// <returns></returns>
        public static bool CheckIntersect(Round r, LineSegment ls)
        {
            return CheckIntersect(ls, r);
        }

        /// <summary>
        /// 检查线段和矩形是否相交
        /// </summary>
        /// <param name="ls"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static bool CheckIntersect(LineSegment ls, Rect rect)
        {
            throw new Exception($"暂未实现");
        }

        /// <summary>
        /// 检查线段和矩形是否相交
        /// </summary>
        /// <param name="r"></param>
        /// <param name="ls"></param>
        /// <returns></returns>
        public static bool CheckIntersect(Rect rect, LineSegment ls)
        {
            return CheckIntersect(ls, rect);
        }
    }
}
