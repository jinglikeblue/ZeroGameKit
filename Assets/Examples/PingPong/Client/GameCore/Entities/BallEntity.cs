using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPong
{
    public struct BallEntity
    {
        /// <summary>
        /// 位置
        /// </summary>
        public PositionComponent position;

        /// <summary>
        /// 速度
        /// </summary>
        public SpeedComponent speed;
    }
}
