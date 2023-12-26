namespace Jing.FixedPointNumber
{
    /// <summary>
    /// 二维坐标点
    /// </summary>
    public struct Vector2
    {
        public Number x;

        public Number y;

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
