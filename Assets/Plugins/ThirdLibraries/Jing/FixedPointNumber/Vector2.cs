namespace Jing.FixedPointNumber
{
    /// <summary>
    /// 二维坐标点
    /// </summary>
    public struct Vector2
    {
        public readonly static Vector2 ZERO = new Vector2((Number)0, (Number)0);
        public readonly static Vector2 ONE = new Vector2((Number)1, (Number)1);

        public Number x;
        public Number y;

        public Number HalfX => x >> 1;
        public Number HalfY => y >> 1;

        public Number DoubleX => x << 1;
        public Number DoubleY => y << 1;

        public Vector2(Number x, Number y)
        {
            this.x = x;
            this.y = y;
        }

        #region override operator
        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x + a.x, a.y + b.y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x - a.x, a.y - b.y);
        }

        public static Vector2 operator *(Vector2 a, Number b)
        {
            return new Vector2(a.x * b, a.y * b);
        }

        public static Vector2 operator *(Number b, Vector2 a)
        {
            return new Vector2(a.x * b, a.y * b);
        }

        public static Vector2 operator /(Vector2 a, Number b)
        {
            return new Vector2(a.x / b, a.y / b);
        }

        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return (a.x == b.x && a.y == b.y) ? true : false;
        }

        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return !(a == b);
        }
        public override bool Equals(object other)
        {
            if (!(other is Vector2)) return false;

            return Equals((Vector2)other);
        }
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2);
        }
        #endregion

        public bool Equals(Vector2 other)
        {
            return x == other.x && y == other.y;
        }

        public override string ToString()
        {
            return $"(x={x},y={y})";
        }
    }
}
