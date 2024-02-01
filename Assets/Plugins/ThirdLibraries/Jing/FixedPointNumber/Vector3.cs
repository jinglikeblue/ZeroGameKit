namespace Jing.FixedPointNumber
{
    /// <summary>
    /// 三维坐标点
    /// </summary>
    public struct Vector3
    {
        public readonly static Vector3 ZERO = new Vector3((Number)0, (Number)0, (Number)0);
        public readonly static Vector3 ONE = new Vector3((Number)1, (Number)1, (Number)1);

        public Number x;
        public Number y;
        public Number z;

        public Number HalfX => x >> 1;
        public Number HalfY => y >> 1;
        public Number HalfZ => z >> 1;

        public Number DoubleX => x << 1;
        public Number DoubleY => y << 1;
        public Number DoubleZ => z << 1;

        public Vector3(Number x, Number y, Number z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public bool Equals(Vector3 other)
        {
            return x == other.x && y == other.y && z == other.z;
        }

        #region override operator
        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x + a.x, a.y + b.y, a.z + b.z);
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x - a.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3 operator *(Vector3 a, Number b)
        {
            return new Vector3(a.x * b, a.y * b, a.z * b);
        }

        public static Vector3 operator *(Number b, Vector3 a)
        {
            return new Vector3(a.x * b, a.y * b, a.z * b);
        }

        public static Vector3 operator /(Vector3 a, Number b)
        {
            return new Vector3(a.x / b, a.y / b, a.z / b);
        }

        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return (a.x == b.x && a.y == b.y && a.z == b.z) ? true : false;
        }

        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return !(a == b);
        }
        public override bool Equals(object other)
        {
            if (!(other is Vector3)) return false;

            return Equals((Vector3)other);
        }
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
        }
        #endregion

        public override string ToString()
        {
            return $"(x={x},y={y},z={z})";
        }
    }
}
