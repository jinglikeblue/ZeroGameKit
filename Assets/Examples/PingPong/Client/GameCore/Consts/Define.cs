using Jing.FixedPointNumber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPong
{
    public static class Define
    {
        /// <summary>
        /// 球的初始位置
        /// </summary>
        public static readonly Vector2 BALL_INITIAL_POSITION = new Vector2(0, 0);

        /// <summary>
        /// 玩家的初始位置
        /// </summary>
        public static readonly Vector2[] PLAYER_INITIAL_POSITION = new Vector2[] { new Vector2(0, 0), new Vector2(0, 0) };
    }
}
