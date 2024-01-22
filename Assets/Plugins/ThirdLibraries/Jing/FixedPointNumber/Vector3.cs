namespace Jing.FixedPointNumber
{
    /// <summary>
    /// 三维坐标点
    /// </summary>
    public struct Vector3
    {
        public readonly static Vector3 ZERO = new Vector3(0, 0, 0);
        public readonly static Vector3 ONE = new Vector3(1, 1, 1);

        public Number x;
        public Number y;
        public Number z;       

        public Vector3(float x, float y, float z)
        {
            this.x = Number.CreateFromFloat(x);
            this.y = Number.CreateFromFloat(y);
            this.z = Number.CreateFromFloat(z);
        }

        public Vector3(Number x, Number y, Number z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
