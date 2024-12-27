using System;

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
        public static readonly Number PI = new Number(31416, 10000);

        /// <summary>
        /// 2π
        /// </summary>
        public static readonly Number TwoPI = PI * 2;

        /// <summary>
        /// π/2
        /// </summary>
        public static readonly Number HalfPI = PI / 2;

        /// <summary>
        /// 角度:360
        /// </summary>
        public static readonly Number Degree360 = (Number)360;

        /// <summary>
        /// 角度:180
        /// </summary>
        public static readonly Number Degree180 = (Number)180;

        /// <summary>
        /// 角度转弧度的系数： 弧度 = 角度 * 该变量
        /// </summary>
        public static readonly Number Deg2Rad = PI / 180;

        /// <summary>
        /// 弧度转角度的系数： 角度 = 弧度 * 该变量
        /// </summary>
        public static readonly Number Rad2Deg = 180 / PI;

        /// <summary>
        /// 绝对值
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Number Abs(Number n)
        {
            return n.Raw < 0 ? Number.CreateFromRaw(-n.Raw) : n;
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
            return Number.CreateFromRaw((n.Raw + Number.FRACTION_MASK) & Number.INTEGER_MASK);
        }

        /// <summary>
        /// 向下取整
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Number Floor(Number n)
        {
            return Number.CreateFromRaw(n.Raw & Number.INTEGER_MASK);
        }

        /// <summary>
        /// 四舍五入取整
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Number Round(Number n)
        {
            return Number.CreateFromRaw((n.Raw + (Number.FRACTION_RANGE >> 1)) & ~Number.FRACTION_MASK);
        }

        /// <summary>
        /// 通过使用舍入到最接近的约定，将数字舍入到指定的小数位数。
        /// </summary>
        /// <param name="n"></param>
        /// <param name="digits">小数位数，最小为0，最大为4</param>
        /// <returns></returns>
        public static Number Round(Number n, int digits)
        {
            if (digits < 0 || digits > 4)
            {
                throw new ArgumentOutOfRangeException("小数位数只能在0到4之间");
            }

            if (0 == digits)
            {
                return Round(n);
            }

            if (4 == digits)
            {
                return n;
            }

            Number scale = Pow((Number)10, digits);
            return Round(n * scale) / scale;
        }

        /// <summary>
        /// 返回指定数字的指定次幂。
        /// </summary>
        /// <param name="x">要乘幂的数</param>
        /// <param name="y">指定幂的数</param>
        /// <returns>数字 x 的 y 次幂</returns>
        public static Number Pow(Number x, int y)
        {
            if (y == 0)
            {
                return Number.ONE;
            }

            //幂可以是负数，例如，x的负y次幂可表示为1/x的y次幂。
            if (y < 0)
            {
                x = 1 / x;
                y = System.Math.Abs(y);
            }

            var result = x;
            while (--y > 0)
            {
                result *= x;
            }
            return result;
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

        #region 通过弧度计算三角函数

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
        /// 计算给定弧度的正切值
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static Number Tan(Number radian)
        {
            return TanTable.TanByRadian(radian);
        }

        /// <summary>
        /// 计算给定弧度的余切值
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static Number Cot(Number radian)
        {
            return 1 / TanTable.TanByRadian(radian);
        }

        #endregion

        #region 通过角度计算三角函数

        /// <summary>
        /// 计算给定角度的正弦值
        /// </summary>
        /// <param name="degree">给定的角度</param>
        /// <returns>角度对应的正弦值</returns>
        public static Number SinByDegree(Number degree)
        {
            return SinCosTable.SinByDegree(degree);
        }

        /// <summary>
        /// 计算给定角度的余弦值
        /// </summary>
        /// <param name="degree">给定的角度</param>
        /// <returns>角度对应的余弦值</returns>
        public static Number CosByDegree(Number degree)
        {
            return SinCosTable.CosByDegree(degree);
        }

        /// <summary>
        /// 计算给定角度的正切值
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static Number TanByDegree(Number degree)
        {
            return TanTable.TanByDegree(degree);
        }

        /// <summary>
        /// 计算给定角度的余切值
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static Number CotByDegree(Number degree)
        {
            return 1 / TanTable.TanByDegree(degree);
        }

        #endregion

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
