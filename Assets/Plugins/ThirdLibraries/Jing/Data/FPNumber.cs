/// <summary>
/// 定点数
/// fixed-point number
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

    /// <summary>
    /// 原始数据:前面的表示整数，后8位为小数
    /// </summary>
    public long Raw { get; private set; }

    public FPNumber(int integer)
    {
        Raw = integer << FRACTIONAL_BIT_COUNT;
    }

    private FPNumber(long raw)
    {
        Raw = raw;
    }

    /// <summary>
    /// 设置原始数据
    /// </summary>
    /// <param name="raw"></param>
    public void SetRaw(long raw)
    {
        Raw = raw;
    }

    /// <summary>
    /// 是否是整数
    /// </summary>
    /// <returns></returns>
    public bool IsInteger()
    {
        return (Raw & FRACTION_MASK) == 0;
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
        var str = string.Format("0:N4", floatingPointNumber);
        var nd = str.Split('.');
        return Create(int.Parse(nd[0]), int.Parse(nd[1]));
    }

    public static FPNumber Create(float floatingPointNumber)
    {
        return Create((double)floatingPointNumber);
    }

    public static FPNumber CreateFromRaw(long raw)
    {
        return new FPNumber(raw);
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

    #endregion
}

