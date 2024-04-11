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
        /// 是否使用BigInteger提高运算精度。
        /// 为true时会牺牲一定的性能，但是不会造成数学运算时数据溢出的情况。
        /// 为false时，运算性能会提高，但是数值过大时可能造成数学运算时数据溢出。
        /// 建议没有性能瓶颈时，设置为true，提高精度。
        /// </summary>
        public const bool IS_USE_BIG_INTEGER = true;

        /// <summary>
        /// 总的位数
        /// </summary>
        public const int TOTAL_BIT_COUNT = sizeof(long) * 8;

        /// <summary>
        /// 用于保存小数的位数
        /// </summary>
        public const int FRACTIONAL_BIT_COUNT = 32;

        /// <summary>
        /// 用于保存整数的位数
        /// </summary>
        public const int INTEGER_BIT_COUNT = TOTAL_BIT_COUNT - FRACTIONAL_BIT_COUNT;

        public const long FRACTION_MASK = (long)(ulong.MaxValue >> INTEGER_BIT_COUNT);
        public const long INTEGER_MASK = -1L & ~FRACTION_MASK;
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
            return ToString(d);
        }

        public int ToInt()
        {
            return (int)(Raw >> FRACTIONAL_BIT_COUNT);
        }

        public short ToShort()
        {
            return (short)ToInt();
        }

        public float ToFloat()
        {
            return (float)ToDouble();
        }

        public double ToDouble()
        {
            return (Raw >> FRACTIONAL_BIT_COUNT) + (Raw & FRACTION_MASK) / (double)FRACTION_RANGE;
        }

        /// <summary>
        /// 转换为二进制
        /// </summary>
        /// <returns></returns>
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

        public static bool operator <(Number a, Number b)
        {
            return a.Raw < b.Raw;
        }

        public static bool operator <(int a, Number b)
        {
            return new Number(a) < b;
        }

        public static bool operator <(Number a, int b)
        {
            return a < new Number(b);
        }

        #endregion

        #region override operator >

        public static bool operator >(Number a, Number b)
        {
            return a.Raw > b.Raw;
        }

        public static bool operator >(int a, Number b)
        {
            return new Number(a) > b;
        }

        public static bool operator >(Number a, int b)
        {
            return a > new Number(b);
        }

        #endregion

        #region override operator <=

        public static bool operator <=(Number a, Number b)
        {
            return a.Raw <= b.Raw;
        }

        public static bool operator <=(int a, Number b)
        {
            return new Number(a) <= b;
        }

        public static bool operator <=(Number a, int b)
        {
            return a <= new Number(b);
        }

        #endregion

        #region override operator >=

        public static bool operator >=(Number a, Number b)
        {
            return a.Raw >= b.Raw;
        }

        public static bool operator >=(int a, Number b)
        {
            return new Number(a) >= b;
        }

        public static bool operator >=(Number a, int b)
        {
            return a >= new Number(b);
        }

        #endregion

        #region override operator ==

        public static bool operator ==(Number a, Number b)
        {
            return a.Raw == b.Raw;
        }

        public static bool operator ==(int a, Number b)
        {
            return new Number(a) == b;
        }

        public static bool operator ==(Number a, int b)
        {
            return a == new Number(b);
        }

        #endregion

        #region override operator !=

        public static bool operator !=(Number a, Number b)
        {
            return a.Raw != b.Raw;
        }

        public static bool operator !=(Number a, int b)
        {
            return a != new Number(b);
        }

        public static bool operator !=(int a, Number b)
        {
            return new Number(a) != b;
        }

        #endregion

        public override bool Equals(object obj)
        {
            return obj != null && GetType() == obj.GetType() && this == (Number)obj;
        }

        #region override operator +

        public static Number operator +(Number a, Number b)
        {
            return new Number(a.Raw + b.Raw);
        }

        public static Number operator +(Number a, int b)
        {
            return a + new Number(b);
        }

        public static Number operator +(int a, Number b)
        {
            return new Number(a) + b;
        }

        #endregion

        #region override operator -

        public static Number operator -(Number a, Number b)
        {
            return new Number(a.Raw - b.Raw);
        }

        public static Number operator -(Number a, int b)
        {
            return a - new Number(b);
        }

        public static Number operator -(int a, Number b)
        {
            return new Number(a) - b;
        }

        #endregion

        #region override operator *

        public static Number operator *(Number a, Number b)
        {
            if (IS_USE_BIG_INTEGER)
            {
                var bigNumber = (BigInteger)a.Raw * b.Raw;
                bigNumber += FRACTION_RANGE >> 1;
                bigNumber >>= FRACTIONAL_BIT_COUNT;
                return new Number(ConvertBigIntegerToLong(ref bigNumber));
            }

            return new Number((a.Raw * b.Raw + (FRACTION_RANGE >> 1)) >> FRACTIONAL_BIT_COUNT);
        }

        public static Number operator *(Number a, int b)
        {
            return a * new Number(b);
        }

        public static Number operator *(int a, Number b)
        {
            return new Number(a) * b;
        }

        #endregion

        #region override operator /

        public static Number operator /(Number a, Number b)
        {
            if (IS_USE_BIG_INTEGER)
            {
                var bigNumber = ((BigInteger)a.Raw) << FRACTIONAL_BIT_COUNT;
                bigNumber /= b.Raw;                
                return new Number(ConvertBigIntegerToLong(ref bigNumber));
            }

            return new Number((a.Raw << FRACTIONAL_BIT_COUNT) / b.Raw);
        }

        public static Number operator /(Number a, int b)
        {
            return a / new Number(b);
        }

        public static Number operator /(int a, Number b)
        {
            return new Number(a) / b;
        }

        #endregion

        #region override operator %

        public static Number operator %(Number a, Number b)
        {
            return new Number(
                a.Raw == MIN_VALUE & b.Raw == -1 ? 0 : a.Raw % b.Raw);
        }

        public static Number operator %(Number a, int b)
        {
            return a % new Number(b);
        }

        public static Number operator %(int a, Number b)
        {
            return new Number(a) % b;
        }

        #endregion


        #region override operator <<

        public static Number operator <<(Number a, int b)
        {
            return new Number(a.Raw << b);
        }

        #endregion

        #region override operator >>

        public static Number operator >>(Number a, int b)
        {
            return new Number(a.Raw >> b);
        }

        #endregion


        public static Number operator -(Number a)
        {
            a.Raw = -a.Raw;
            return a;
        }

        #region override operator explicit 显示转换规则

        public static explicit operator int(Number a)
        {
            return a.ToInt();
        }

        public static explicit operator Number(int a)
        {
            return new Number(a);
        }

        #endregion

        #region override operator implicit 隐式转换规则

        #endregion

        public override int GetHashCode()
        {
            return _raw.GetHashCode();
        }

        #endregion

        public string Info
        {
            get { return $"[Number]  VALUE:{ToString()}  RAW:{_raw}  BINARY:{System.Convert.ToString(_raw, 2).PadLeft(TOTAL_BIT_COUNT, '-')}"; }
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