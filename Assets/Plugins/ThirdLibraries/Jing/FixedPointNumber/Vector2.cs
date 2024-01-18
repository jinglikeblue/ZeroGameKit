namespace Jing.FixedPointNumber
{
    /// <summary>
    /// 二维坐标点
    /// </summary>
    public struct Vector2
    {
        public readonly static Vector2 ZERO = new Vector2(0, 0);
        public readonly static Vector2 ONE = new Vector2(1, 1);

        public Number x;
        public Number y;

        public Number HalfX => x >> 1;
        public Number HalfY => y >> 1;

        public Number DoubleX => x << 1;
        public Number DoubleY => y << 1;



        public Vector2(int x, int y)
        {
            this.x = new Number(x);
            this.y = new Number(y);
        }

        public Vector2(Number x, Number y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
