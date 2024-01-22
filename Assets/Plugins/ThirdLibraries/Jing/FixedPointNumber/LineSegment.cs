namespace Jing.FixedPointNumber
{
    /// <summary>
    /// 线段
    /// </summary>
    public class LineSegment
    {
        Number _startX;
        Number _startY;
        Number _endX;
        Number _endY;

        /// <summary>
        /// 起点的X坐标
        /// </summary>
        public Number startX => _startX;

        /// <summary>
        /// 起点的Y坐标
        /// </summary>
        public Number startY => _startY;

        /// <summary>
        /// 终点的X坐标
        /// </summary>
        public Number endX => _endX;

        /// <summary>
        /// 终点的Y坐标
        /// </summary>
        public Number endY => _endY;

        public LineSegment(Vector2 startPos, Vector2 endPos)
        {
            _startX = startPos.x;
            _startY = startPos.y;
            _endX = endPos.x;
            _endY = endPos.y;
        }

        public LineSegment(Number startX, Number startY, Number endX, Number endY)
        {
            _startX = startX;
            _startY = startY;
            _endX = endX;
            _endY = endY;
        }

        /// <summary>
        /// 线段长度的平方
        /// </summary>
        /// <returns></returns>
        public Number LengthSquared
        {
            get
            {
                var distanceX = _endX - _startX;
                var distanceY = _endY - _startY;
                return (distanceX * distanceX) + (distanceY * distanceY);
            }
        }

        /// <summary>
        /// 线段的长度
        /// </summary>
        public Number Length
        {
            get
            {
                return Math.Sqrt(LengthSquared);
            }
        }
    }
}
