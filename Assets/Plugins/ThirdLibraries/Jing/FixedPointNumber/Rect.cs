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
    }
}
