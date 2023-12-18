using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPong
{
    public struct PlayerEntity
    {
        /// <summary>
        /// ID
        /// </summary>
        public int id;

        /// <summary>
        /// 位置
        /// </summary>
        public PositionComponent position;

        /// <summary>
        /// 速度
        /// </summary>
        public SpeedComponent speed;

        /// <summary>
        /// 大小
        /// </summary>
        public RectSizeComponent size;
    }
}
