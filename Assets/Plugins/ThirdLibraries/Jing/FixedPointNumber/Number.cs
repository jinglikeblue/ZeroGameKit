using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Jing.FixedPointNumber
{
    /// <summary>
    /// 定点数。
    /// fixed-point number。 
    /// </summary>
    public struct Number
    {
        /// <summary>
        /// 总的位数
        /// </summary>
        public const int TOTAL_BIT_COUNT = sizeof(long) * 8;

        /// <summary>
        /// 用于保存小数的位数
        /// 如果整数部分比较大，建议设置为16，小数可以保留到小数点后4位
        /// 如果要和double计算的精度相近，建议设置为32        
        /// </summary>
        public const int FRACTIONAL_BIT_COUNT = 32;

        /// <summary>
        /// 用于保存整数的位数
        /// </summary>
        public const int INTEGER_BIT_COUNT = TOTAL_BIT_COUNT - FRACTIONAL_BIT_COUNT;

        /// <summary>
        /// 小数位的掩码
        /// </summary>
        public const long FRACTION_MASK = (long)(ulong.MaxValue >> INTEGER_BIT_COUNT);

        /// <summary>
        /// 整数位的掩码
        /// </summary>
        public const long INTEGER_MASK = -1L & ~FRACTION_MASK;

        /// <summary>
        /// 小数的值的范围
        /// </summary>
        public const long FRACTION_RANGE = FRACTION_MASK + 1;

        /// <summary>
        /// 最小值
        /// </summary>
        public const long MIN_VALUE = long.MinValue >> FRACTIONAL_BIT_COUNT;

        /// <summary>
        /// 最大值
        /// </summary>
        public const long MAX_VALUE = long.MaxValue >> FRACTIONAL_BIT_COUNT;

        /// <summary>
        /// 最大值
        /// </summary>
        public readonly static Number MAX = new Number(MAX_VALUE);

        /// <summary>
        /// 最小值
        /// </summary>
        public readonly static Number MIN = new Number(MIN_VALUE);

        /// <summary>
        /// -1
        /// </summary>
        public readonly static Number NEGATIVE_ONE = new Number(-1);

        /// <summary>
        /// 0
        /// </summary>
        public readonly static Number ZERO = new Number(0);

        /// <summary>
        /// 1
        /// </summary>
        public readonly static Number ONE = new Number(1);


        long _raw;

        /// <summary>
        /// 原始数据:前面的表示整数，后8位为小数
        /// </summary>
        public long Raw
        {
            set { _raw = value; }

            get { return _raw; }
        }

        #region 静态方法

        /// <summary>
        /// 通过原始数据直接创建定点数
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        public static Number CreateFromRaw(long raw)
        {
            return new Number(raw);
        }

        /// <summary>
        /// 通过浮点数来创建定点数(精度自动四舍五入到4位)。
        /// 相比Number(int numerator, int denominator)，开销更大
        /// </summary>
        /// <param name="floatingPointNumber"></param>
        public static Number CreateFromDouble(double floatingPointNumber)
        {
            var raw = DoubleToRaw(floatingPointNumber);
            return CreateFromRaw(raw);
        }

        /// <summary>
        /// 通过浮点数来创建定点数(精度自动四舍五入到4位)
        /// 相比Number(int numerator, int denominator)，开销更大
        /// </summary>
        /// <param name="floatingPointNumber"></param>
        public static Number CreateFromFloat(float floatingPointNumber)
        {
            var raw = FloatToRaw(floatingPointNumber);
            return CreateFromRaw(raw);
        }

        /// <summary>
        /// 整数转换成Raw
        /// </summary>
        /// <param name="integer"></param>
        /// <returns></returns>
        public static long IntegerToRaw(int integer)
        {
            //先转long这样左移的时候，左边的空间才够
            return (long)integer << FRACTIONAL_BIT_COUNT;
        }

        /// <summary>
        /// 通过分子分母生成Raw
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        public static long NumeratorAndDenominatorToRaw(int numerator, int denominator)
        {
            return IntegerToRaw(numerator) / denominator;
        }

        /// <summary>
        /// 通过浮点数生成Raw
        /// </summary>
        /// <param name="floatingPointNumber"></param>
        /// <returns></returns>
        public static long FloatToRaw(float floatingPointNumber)
        {
            return DoubleToRaw(floatingPointNumber);
        }

        /// <summary>
        /// 通过浮点数生成Raw(字符串方式)
        /// </summary>
        /// <param name="floatingPointNumber"></param>
        /// <returns></returns>
        //public static long DoubleToRaw(double floatingPointNumber)
        //{        
        //    var numberStr = ToString(floatingPointNumber);
        //    var pointPos = numberStr.IndexOf('.');
        //    if (-1 == pointPos)
        //    {
        //        //没有小数点，就是整数
        //        return IntegerToRaw(int.Parse(numberStr));
        //    }

        //    var integersPartStr = numberStr.Substring(0, pointPos);
        //    var decimalsPartStr = numberStr.Substring(pointPos + 1);

        //    //分子
        //    int numerator = int.Parse(integersPartStr + decimalsPartStr);

        //    //分母
        //    int denominator = 10;
        //    for (int i = 1; i < decimalsPartStr.Length; i++)
        //    {
        //        denominator *= 10;
        //    }

        //    return NumeratorAndDenominatorToRaw(numerator, denominator);
        //}

        /// <summary>
        /// 通过浮点数生成Raw
        /// </summary>
        /// <param name="floatingPointNumber"></param>
        /// <returns></returns>
        public static long DoubleToRaw(double floatingPointNumber)
        {
            if (0 == floatingPointNumber)
            {
                return IntegerToRaw(0);
            }

            //保留4位精度
            var roundFloatingPointNumber = System.Math.Round(floatingPointNumber, 4);
            //拿到绝对值
            var absolute = System.Math.Abs(roundFloatingPointNumber);
            int denominator = 1;

            while (denominator < 10000 && (absolute * denominator) % 1 > 0)
            {
                denominator *= 10;
            }

            //分子
            int numerator = System.Convert.ToInt32(roundFloatingPointNumber * denominator);

            return NumeratorAndDenominatorToRaw(numerator, denominator);
        }

        #endregion

        private Number(long raw)
        {
            _raw = raw;
        }

        /// <summary>
        /// 通过整数创建定点数
        /// </summary>
        /// <param name="integer"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Number(int integer)
        {
            _raw = IntegerToRaw(integer);
        }

        /// <summary>
        /// 通过指定的分子和分母来创建定点数
        /// </summary>
        /// <param name="numerator">分子</param>
        /// <param name="denominator">分母</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Number(int numerator, int denominator)
        {
            _raw = NumeratorAndDenominatorToRaw(numerator, denominator);
        }

        /// <summary>
        /// 是否是整数
        /// </summary>
        /// <returns></returns>        
        public bool IsInteger
        {
            get { return (Raw & FRACTION_MASK) == 0; }
        }

        #region 数据输出

        static string ToString(double floatingPointNumber)
        {
            var str = floatingPointNumber.ToString("F4").TrimEnd('0').TrimEnd('.');
            return str;
        }

        public override string ToString()
        {
            double d = ToDouble();
            return d.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ToInt()
        {
            return (int)(Raw >> FRACTIONAL_BIT_COUNT);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ToShort()
        {
            return (short)ToInt();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ToFloat()
        {
            return (float)ToDouble();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ToDouble()
        {
            return (double)_raw / ONE.Raw;
            //return (Raw >> FRACTIONAL_BIT_COUNT) + (Raw & FRACTION_MASK) / (double)FRACTION_RANGE;
        }

        /// <summary>
        /// 转换为二进制
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToBinary(bool isPadLeft = true)
        {
            string binary = Convert.ToString(Raw, 2);
            if (isPadLeft)
            {
                binary = binary.PadLeft(TOTAL_BIT_COUNT, '0');
            }
            return binary;
        }

        /// <summary>
        /// 获取整数部分
        /// </summary>
        public Number IntegerPart => new Number(Raw & INTEGER_MASK);

        /// <summary>
        /// 获取小数部分
        /// </summary>        
        public Number FractionalPart => new Number(Raw & FRACTION_MASK);

        #endregion

        #region 重写运算符

        #region override operator <
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(Number a, Number b)
        {
            return a.Raw < b.Raw;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(int a, Number b)
        {
            return new Number(a) < b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(Number a, int b)
        {
            return a < new Number(b);
        }

        #endregion

        #region override operator >
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(Number a, Number b)
        {
            return a.Raw > b.Raw;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(int a, Number b)
        {
            return new Number(a) > b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(Number a, int b)
        {
            return a > new Number(b);
        }

        #endregion

        #region override operator <=

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(Number a, Number b)
        {
            return a.Raw <= b.Raw;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(int a, Number b)
        {
            return new Number(a) <= b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(Number a, int b)
        {
            return a <= new Number(b);
        }

        #endregion

        #region override operator >=

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(Number a, Number b)
        {
            return a.Raw >= b.Raw;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(int a, Number b)
        {
            return new Number(a) >= b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(Number a, int b)
        {
            return a >= new Number(b);
        }

        #endregion

        #region override operator ==

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Number a, Number b)
        {
            return a.Raw == b.Raw;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(int a, Number b)
        {
            return new Number(a) == b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Number a, int b)
        {
            return a == new Number(b);
        }

        #endregion

        #region override operator !=

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Number a, Number b)
        {
            return a.Raw != b.Raw;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Number a, int b)
        {
            return a != new Number(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(int a, Number b)
        {
            return new Number(a) != b;
        }

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return obj != null && GetType() == obj.GetType() && this == (Number)obj;
        }

        #region override operator +

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Number operator +(Number a, Number b)
        {
            return new Number(a.Raw + b.Raw);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Number operator +(Number a, int b)
        {
            return a + new Number(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Number operator +(int a, Number b)
        {
            return new Number(a) + b;
        }

        #endregion

        #region override operator -

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Number operator -(Number a, Number b)
        {
            return new Number(a.Raw - b.Raw);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Number operator -(Number a, int b)
        {
            return a - new Number(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Number operator -(int a, Number b)
        {
            return new Number(a) - b;
        }

        #endregion

        #region override operator *

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Number operator *(Number a, Number b)
        {
            if (a == ONE) return b;
            else if (b == ONE) return a;

            var xlo = (ulong)(a.Raw & FRACTION_MASK);
            var xhi = a.Raw >> FRACTIONAL_BIT_COUNT;
            var ylo = (ulong)(b.Raw & FRACTION_MASK);
            var yhi = b.Raw >> FRACTIONAL_BIT_COUNT;

            var lolo = xlo * ylo;
            var lohi = (long)xlo * yhi;
            var hilo = xhi * (long)ylo;
            var hihi = xhi * yhi;

            var loResult = lolo >> FRACTIONAL_BIT_COUNT;
            var midResult1 = lohi;
            var midResult2 = hilo;
            var hiResult = hihi << FRACTIONAL_BIT_COUNT;

            return new Number((long)loResult + midResult1 + midResult2 + hiResult);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Number operator *(Number a, int b)
        {
            return a * new Number(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Number operator *(int a, Number b)
        {
            return new Number(a) * b;
        }

        #endregion

        #region override operator /

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CountLeadingZeroes(ulong x)
        {
            var result = 0;
            while ((x & 0xF000000000000000) == 0)
            { result += 4; x <<= 4; }
            while ((x & 0x8000000000000000) == 0)
            { result += 1; x <<= 1; }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Number operator /(Number a, Number b)
        {
            if (b == ONE) return a;

            var xl = a.Raw;
            var yl = b.Raw;

            if (yl == 0)
            {
                return MAX;
            }

            if (xl == 0)
            {
                return ZERO;
            }

            var remainder = (ulong)(xl >= 0 ? xl : -xl);
            var divider = (ulong)(yl >= 0 ? yl : -yl);
            var quotient = 0UL;
            var bitPos = FRACTIONAL_BIT_COUNT + 1;

            // If the divider is divisible by 2^n, take advantage of it.
            while ((divider & 0xF) == 0 && bitPos >= 4)
            {
                divider >>= 4;
                bitPos -= 4;
            }

            while (remainder != 0 && bitPos >= 0)
            {
                var shift = CountLeadingZeroes(remainder);
                if (shift > bitPos)
                {
                    shift = bitPos;
                }
                remainder <<= shift;
                bitPos -= shift;

                var div = remainder / divider;
                remainder %= divider;
                quotient += div << bitPos;

                //// Detect overflow
                //if ((div & ~(0xFFFFFFFFFFFFFFFF >> bitPos)) != 0)
                //{
                //    return ((xl ^ yl) & MIN_VALUE) == 0 ? MaxValue : MinValue;
                //}

                remainder <<= 1;
                --bitPos;
            }

            // rounding
            ++quotient;
            var result = (long)(quotient >> 1);
            if (((xl ^ yl) & long.MinValue) != 0)
            {
                result = -result;
            }

            return new Number(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Number operator /(Number a, int b)
        {
            return a / new Number(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Number operator /(int a, Number b)
        {
            return new Number(a) / b;
        }

        #endregion

        #region override operator %
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Number operator %(Number a, Number b)
        {
            return new Number(
                a.Raw == MIN_VALUE & b.Raw == -1 ? 0 : a.Raw % b.Raw);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Number operator %(Number a, int b)
        {
            return a % new Number(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Number operator %(int a, Number b)
        {
            return new Number(a) % b;
        }

        #endregion


        #region override operator <<
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Number operator <<(Number a, int b)
        {
            return new Number(a.Raw << b);
        }

        #endregion

        #region override operator >>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Number operator >>(Number a, int b)
        {
            return new Number(a.Raw >> b);
        }

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Number operator -(Number a)
        {
            a.Raw = -a.Raw;
            return a;
        }

        #region override operator explicit 显示转换规则
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator int(Number a)
        {
            return a.ToInt();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Number(int a)
        {
            return new Number(a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Number(float a)
        {
            throw new Exception("Wrong!");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Number(double a)
        {
            throw new Exception("Wrong!");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Number(long a)
        {
            throw new Exception("Wrong!");
        }

        #endregion

        #region override operator implicit 隐式转换规则

        #endregion
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return _raw.GetHashCode();
        }

        #endregion

        public string Info
        {
            get { return $"[Number]  VALUE:{ToString()}  RAW:{_raw}  BINARY:{Convert.ToString(_raw, 2).PadLeft(TOTAL_BIT_COUNT, '-')}"; }
        }

        /// <summary>
        /// 将BigInteger转换为long
        /// </summary>
        /// <param name="bigIngeter"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static long ConvertBigIntegerToLong(ref BigInteger bigIngeter)
        {
            if (bigIngeter > long.MaxValue)
            {
                return long.MaxValue;
            }

            if (bigIngeter < long.MinValue)
            {
                return long.MinValue;
            }

            return (long)bigIngeter;
        }
    }
}