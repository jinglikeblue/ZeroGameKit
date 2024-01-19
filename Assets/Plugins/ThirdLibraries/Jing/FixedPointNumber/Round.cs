using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jing.FixedPointNumber
{
    /// <summary>
    /// 圆形
    /// </summary>
    public struct Round
    {
        /// <summary>
        /// 圆心坐标X
        /// </summary>
        Number _x;

        /// <summary>
        /// 圆心坐标Y
        /// </summary>
        Number _y;

        /// <summary>
        /// 半径
        /// </summary>
        Number _radius;

        /// <summary>
        /// 圆心坐标X
        /// </summary>
        public Number x
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        /// <summary>
        /// 圆心坐标Y
        /// </summary>
        public Number y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }

        /// <summary>
        /// 半径
        /// </summary>
        public Number radius
        {
            get
            {
                return _radius;
            }
            set
            {
                _radius = value;
            }
        }

        /// <summary>
        /// 直径
        /// </summary>
        public Number diameter
        {
            get
            {
                return _radius << 1;
            }
        }

        public Round(int x, int y, int radius)
        {
            _x = new Number(x);
            _y = new Number(y);
            _radius = new Number(radius);
        }

        public Round(Number x, Number y, Number radius)
        {
            _x = x;
            _y = y;
            _radius = radius;
        }

        /// <summary>
        /// 是否包含目标点
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(Vector2 point)
        {
            var distanceX = point.x - _x;
            var squaredX = distanceX * distanceX;
            var distanceY = point.y - _y;
            var squaredY = distanceY * distanceY;
            if ((squaredX + squaredY) > _radius * _radius)
            {
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            return $"[x={_x},y={y},r={radius}]";
        }
    }
}
