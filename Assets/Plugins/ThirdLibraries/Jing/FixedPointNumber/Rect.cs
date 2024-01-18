using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jing.FixedPointNumber
{
    /// <summary>
    /// 矩形
    /// </summary>
    public struct Rect
    {
        Number _x;
        Number _y;
        Number _w;
        Number _h;

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

        public Number width
        {
            get
            {
                return _w;
            }
            set
            {
                _w = value;
            }
        }

        public Number height
        {
            get
            {
                return _h;
            }
            set
            {
                _h = value;
            }
        }

        public Rect(Vector2 position, Vector2 size)
        {
            _x = position.x;
            _y = position.y;
            _w = size.x;
            _h = size.y;
        }

        public Rect(Number x, Number y, Number width, Number height)
        {
            _x = x;
            _y = y;
            _w = width;
            _h = height;
        }

        public Rect(int x, int y, int width, int height)
        {
            _x = new Number(x);
            _y = new Number(y);
            _w = new Number(width);
            _h = new Number(height);
        }

        /// <summary>
        /// 矩形的中心点
        /// </summary>
        public Vector2 center
        {
            get
            {
                return new Vector2(_x + (_w >> 1), _y + (_h >> 1));
            }
            set
            {
                var centerX = value.x;
                var centerY = value.y;
                _x = centerX - (_w >> 1);
                _y = centerY - (_h >> 1);
            }
        }

        public Number left
        {
            get
            {
                return _x;
            }
        }

        public Number right
        {
            get
            {
                return _x + _w;
            }
        }

        public Number top
        {
            get
            {
                return _y;
            }
        }

        public Number bottom
        {
            get
            {
                return _y + _h;
            }
        }

        /// <summary>
        /// 矩形是否包含目标点
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(Vector2 point)
        {
            return (point.x >= left) && (point.x < right) && (point.y >= top) && (point.y < bottom);
        }

        public override string ToString()
        {
            return $"[x={x},y={y},w={width},h={height}]";
        }
    }
}
