namespace Jing.FixedPointNumber
{
    /// <summary>
    /// 三维坐标点
    /// </summary>
    public struct Vector3
    {
        public static Vector3 zero { get; private set; } = new Vector3(0, 0, 0);
        public static Vector3 one { get; private set; } = new Vector3(1, 1, 1);

        public Number x;
        public Number y;
        public Number z;       

        public Vector3(float x, float y, float z)
        {
            this.x = new Number(x);
            this.y = new Number(y);
            this.z = new Number(z);
        }

        public Vector3(Number x, Number y, Number z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
