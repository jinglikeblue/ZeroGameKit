using Jing.FixedPointNumber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPong
{
    /// <summary>
    /// 世界实体
    /// </summary>
    public class WorldEntity
    {
        /// <summary>
        /// 球
        /// </summary>
        public BallEntity ball;

        /// <summary>
        /// 玩家
        /// </summary>
        public PlayerEntity[] players;

        /// <summary>
        /// 大小
        /// </summary>
        public RectSizeComponent size;

        /// <summary>
        /// 状态
        /// </summary>
        public EWorldState state;

        /// <summary>
        /// 赢家
        /// </summary>
        public int winner;
    }
}
