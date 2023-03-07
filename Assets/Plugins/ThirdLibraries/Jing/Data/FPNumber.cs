using System;
/// <summary>
/// 定点数。
/// fixed-point number。
/// 使用建议：通过静态方法Create创建定点数，效率略高于使用new关键字实例化
/// </summary>
public struct FPNumber
{
    /// <summary>
    /// 总的位数
    /// </summary>
    const int TOTAL_BIT_COUNT = sizeof(long) * 8;

    /// <summary>
    /// 用于保存小数的位数，16位可以保留小数点后4位的进度
    /// </summary>
    const int FRACTIONAL_BIT_COUNT = 16;

    /// <summary>
    /// 用于保存整数的位数
    /// </summary>
    const int INTEGER_BIT_COUNT = TOTAL_BIT_COUNT - FRACTIONAL_BIT_COUNT;

    const long FRACTION_MASK = (long)(ulong.MaxValue >> INTEGER_BIT_COUNT);
    const long INTEGER_MASK = -1L & ~FRACTION_MASK;
    const long FRACTION_RANGE = FRACTION_MASK + 1;

    /// <summary>
    /// 最小值
    /// </summary>
    public const long MIN_VALUE = long.MinValue >> TOTAL_BIT_COUNT;

    /// <summary>
    /// 最大值
    /// </summary>
    public const long MAX_VALUE = long.MaxValue >> TOTAL_BIT_COUNT;

    long _raw;

    /// <summary>
    /// 原始数据:前面的表示整数，后8位为小数
    /// </summary>
    public long Raw
    {
        set
        {
            _raw = value;
        }

        get
        {
            return _raw;
        }
    }

    public string Info
    {
        get
        {
            return $"[FPNumber]  VALUE:{ToString()}  RAW:{_raw}  BINARY:{System.Convert.ToString(_raw, 2).PadLeft(64, '-')}";
        }
    }

    public FPNumber(long raw)
    {
        _raw = raw;
    }

    public FPNumber(int integer)
    {
        _raw = integer << FRACTIONAL_BIT_COUNT;
    }

    public FPNumber(int numerator, int denominator)
    {
        var temp = Create(numerator, denominator);
        _raw = temp.Raw;
    }

    public FPNumber(double floatingPointNumber)
    {
        var temp = Create(floatingPointNumber);
        _raw = temp.Raw;
    }

    public FPNumber(float floatingPointNumber)
    {
        var temp = Create(floatingPointNumber);
        _raw = temp.Raw;
    }

    /// <summary>
    /// 是否是整数
    /// </summary>
    /// <returns></returns>
    public bool IsInteger()
    {
        return (Raw & FRACTION_MASK) == 0;
    }

    /// <summary>
    /// 通过原始数据直接创建定点数
    /// </summary>
    /// <param name="raw"></param>
    /// <returns></returns>
    public static FPNumber Create(long raw)
    {
        return new FPNumber(raw);
    }

    public static FPNumber Create(int integer)
    {
        return new FPNumber(integer << FRACTIONAL_BIT_COUNT);
    }

    public static FPNumber Create(int numerator, int denominator)
    {
        return new FPNumber(numerator) / denominator;
    }

    public static FPNumber Create(double floatingPointNumber)
    {
        ////数学方式获得分母
        //{
        //    int denominator = 0;
        //    while (num % 1 > 0)
        //    {
        //        num *= 10;
        //        bits++;
        //    }
        //    return bits;
        //}


        var str = ToString(floatingPointNumber);
        //从小数点分割整数部分和小数部分
        var nd = str.Split('.');
        //整数部分的值
        int integerValue = int.Parse(nd[0]);
        if (1 == nd.Length)
        {
            //按照整数处理
            return Create(integerValue);
        }

        //得到分子
        int numerator = int.Parse(nd[0] + nd[1]);

        #region 得到分母
        //首先获取小数点后有多少位
        var digitsAfterDecimalPoint = str.Length - str.IndexOf('.') - 1;
        var denominator = 10;

        //这里不用Math.Pow因为返回的是double
        while (--digitsAfterDecimalPoint > 0)
        {
            denominator *= 10;
        }
        #endregion

        return Create(numerator, denominator);
    }

    public static FPNumber Create(float floatingPointNumber)
    {
        return Create((double)floatingPointNumber);
    }

    public int ToInt()
    {
        return (int)(Raw >> FRACTIONAL_BIT_COUNT);
    }

    public short ToShort()
    {
        return (short)ToInt();
    }

    public double ToDouble()
    {
        return (Raw >> FRACTIONAL_BIT_COUNT) + (Raw & FRACTION_MASK) / (double)FRACTION_RANGE;
    }

    public float ToFloat()
    {
        return (float)ToDouble();
    }

    static string ToString(double floatingPointNumber)
    {
        var t = floatingPointNumber.ToString("F4");
        var str = floatingPointNumber.ToString("F4").TrimEnd('0').TrimEnd('.');
        return str;
    }

    public override string ToString()
    {
        return ToString(ToDouble());
    }



    //强转重写
    //public static explicit operator double(FPNumber a)
    //{
    //    return a.ToDouble();
    //}

    #region 重写运算符

    #region override operator <
    public static bool operator <(FPNumber a, FPNumber b)
    {
        return a.Raw < b.Raw;
    }

    public static bool operator <(int a, FPNumber b)
    {
        return new FPNumber(a) < b;
    }

    public static bool operator <(FPNumber a, int b)
    {
        return a < new FPNumber(b);
    }
    #endregion

    #region override operator >
    public static bool operator >(FPNumber a, FPNumber b)
    {
        return a.Raw > b.Raw;
    }

    public static bool operator >(int a, FPNumber b)
    {
        return new FPNumber(a) > b;
    }

    public static bool operator >(FPNumber a, int b)
    {
        return a > new FPNumber(b);
    }
    #endregion

    #region override operator <=
    public static bool operator <=(FPNumber a, FPNumber b)
    {
        return a.Raw <= b.Raw;
    }

    public static bool operator <=(int a, FPNumber b)
    {
        return new FPNumber(a) <= b;
    }

    public static bool operator <=(FPNumber a, int b)
    {
        return a <= new FPNumber(b);
    }
    #endregion

    #region override operator >=
    public static bool operator >=(FPNumber a, FPNumber b)
    {
        return a.Raw >= b.Raw;
    }
    public static bool operator >=(int a, FPNumber b)
    {
        return new FPNumber(a) >= b;
    }

    public static bool operator >=(FPNumber a, int b)
    {
        return a >= new FPNumber(b);
    }
    #endregion

    #region override operator ==
    public static bool operator ==(FPNumber a, FPNumber b)
    {
        return a.Raw == b.Raw;
    }

    public static bool operator ==(int a, FPNumber b)
    {
        return new FPNumber(a) == b;
    }

    public static bool operator ==(FPNumber a, int b)
    {
        return a == new FPNumber(b);
    }
    #endregion

    #region override operator !=
    public static bool operator !=(FPNumber a, FPNumber b)
    {
        return a.Raw != b.Raw;
    }

    public static bool operator !=(FPNumber a, int b)
    {
        return a != new FPNumber(b);
    }

    public static bool operator !=(int a, FPNumber b)
    {
        return new FPNumber(a) != b;
    }
    #endregion

    public override bool Equals(object obj)
    {
        return obj != null && GetType() == obj.GetType() && this == (FPNumber)obj;
    }

    #region override operator + 
    public static FPNumber operator +(FPNumber a, FPNumber b)
    {
        return new FPNumber(a.Raw + b.Raw);
    }

    public static FPNumber operator +(FPNumber a, int b)
    {
        return a + new FPNumber(b);
    }

    public static FPNumber operator +(int a, FPNumber b)
    {
        return new FPNumber(a) + b;
    }
    #endregion

    #region override operator -
    public static FPNumber operator -(FPNumber a, FPNumber b)
    {
        return new FPNumber(a.Raw - b.Raw);
    }

    public static FPNumber operator -(FPNumber a, int b)
    {
        return a - new FPNumber(b);
    }

    public static FPNumber operator -(int a, FPNumber b)
    {
        return new FPNumber(a) - b;
    }

    #endregion

    #region override operator *
    public static FPNumber operator *(FPNumber a, FPNumber b)
    {
        return new FPNumber((a.Raw * b.Raw + (FRACTION_RANGE >> 1)) >> FRACTIONAL_BIT_COUNT);
    }

    public static FPNumber operator *(FPNumber a, int b)
    {
        return a * new FPNumber(b);
    }

    public static FPNumber operator *(int a, FPNumber b)
    {
        return new FPNumber(a) * b;
    }
    #endregion

    #region override operator /
    public static FPNumber operator /(FPNumber a, FPNumber b)
    {
        return new FPNumber((a.Raw << FRACTIONAL_BIT_COUNT) / b.Raw);
    }

    public static FPNumber operator /(FPNumber a, int b)
    {
        return a / new FPNumber(b);
    }

    public static FPNumber operator /(int a, FPNumber b)
    {
        return new FPNumber(a) / b;
    }
    #endregion

    #region override operator <<
    public static FPNumber operator <<(FPNumber a, int b)
    {
        return new FPNumber(a.Raw << b);
    }
    #endregion

    #region override operator >>
    public static FPNumber operator >>(FPNumber a, int b)
    {
        return new FPNumber(a.Raw >> b);
    }
    #endregion

    public static FPNumber operator -(FPNumber a)
    {
        a.Raw = -a.Raw;
        return a;
    }

    public override int GetHashCode()
    {
        return _raw.GetHashCode();
    }

    #endregion
}

