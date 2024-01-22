using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jing.FixedPointNumber
{
    /// <summary>
    /// 定点数数学处理
    /// </summary>
    public static class Math
    {
        /// <summary>
        /// π
        /// </summary>
        public static Number Pi => new Number(31416, 10000);

        /// <summary>
        /// 2π
        /// </summary>
        public static Number TwoPi => new Number(62832, 10000);

        /// <summary>
        /// π/2
        /// </summary>
        public static Number HalfPi => new Number(15708, 10000);

        /// <summary>
        /// 角度:360
        /// </summary>
        public static Number Degree360 => new Number(360);

        /// <summary>
        /// 角度:180
        /// </summary>
        public static Number Degree180 => new Number(180);

        /// <summary>
        /// 角度转弧度的系数： 弧度 = 角度 * 该变量
        /// </summary>
        public static Number RadianCoefficient => Pi / 180;

        /// <summary>
        /// 弧度转角度的系数： 角度 = 弧度 * 该变量
        /// </summary>
        public static Number DegreeCoefficient => 180 / Pi;

        /// <summary>
        /// 绝对值
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Number Abs(Number n)
        {
            return n.Raw < 0 ? new Number(-n.Raw) : n;
        }

        /// <summary>
        /// 返回一个数字的符号, 指示数字是正数，负数还是零
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Number Sign(Number value)
        {
            if (value > 0)
            {
                return Number.ONE;
            }
            else if (value < 0)
            {
                return Number.NEGATIVE_ONE;
            }
            return Number.ZERO;
        }

        /// <summary>
        /// 返回较大值
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Number Max(Number a, Number b)
        {
            return a > b ? a : b;
        }

        /// <summary>
        /// 返回较小值
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Number Min(Number a, Number b)
        {
            return a < b ? a : b;
        }

        /// <summary>
        /// 向上取整
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Number Ceil(Number n)
        {
            return new Number((n.Raw + Number.FRACTION_MASK) & Number.INTEGER_MASK);
        }

        /// <summary>
        /// 向下取整
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Number Floor(Number n)
        {
            return new Number(n.Raw & Number.INTEGER_MASK);
        }

        /// <summary>
        /// 四舍五入取整
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Number Round(Number n)
        {
            return new Number((n.Raw + (Number.FRACTION_RANGE >> 1)) & ~Number.FRACTION_MASK);
        }

        /// <summary>
        /// 开平方根
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Number Sqrt(Number value)
        {
            if (value.Raw < 0)
                throw new ArgumentOutOfRangeException("value", "Value must be non-negative.");
            if (value.Raw == 0)
                return Number.ZERO;

            return new Number((int)(SqrtULong((ulong)value.Raw << (Number.FRACTIONAL_BIT_COUNT + 2)) + 1) >> 1);
        }

        internal static uint SqrtULong(ulong N)
        {
            ulong x = 1L << ((31 + (Number.FRACTIONAL_BIT_COUNT + 2) + 1) / 2);
            while (true)
            {
                ulong y = (x + N / x) >> 1;
                if (y >= x)
                    return (uint)x;
                x = y;
            }
        }

        /// <summary>
        /// 计算给定弧度的正弦值
        /// </summary>
        /// <param name="radian">给定的弧度</param>
        /// <returns>弧度对应的正弦值</returns>
        public static Number Sin(Number radian)
        {
            return SinCosTable.SinByRadian(radian);
        }

        /// <summary>
        /// 计算给定弧度的余弦值
        /// </summary>
        /// <param name="radian">给定的弧度</param>
        /// <returns>弧度对应的余弦值</returns>
        public static Number Cos(Number radian)
        {
            return SinCosTable.CosByRadian(radian);
        }

        /// <summary>
        /// 将给定的值限定在指定的最小值和最大值之间
        /// </summary>
        /// <param name="value">要限定的值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>如果值小于最小值，则返回最小值；如果值大于最大值，则返回最大值；否则返回原始值</returns>
        public static Number Clamp(Number value, Number min, Number max)
        {
            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }
            else
            {
                return value;
            }
        }
    }
}
