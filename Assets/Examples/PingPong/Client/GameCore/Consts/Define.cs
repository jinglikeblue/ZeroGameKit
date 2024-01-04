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
        /// 场地的大小
        /// </summary>
        public static readonly Vector2 WORLD_SIZE = new Vector2(20, 30);

        /// <summary>
        /// 球的半径大小
        /// </summary>
        public static readonly Number BALL_SIZE = new Number(5, 10);

        /// <summary>
        /// 球的初始位置
        /// </summary>
        public static readonly Vector2 BALL_INITIAL_POSITION = new Vector2(0, 0);

        /// <summary>
        /// 玩家的大小
        /// </summary>
        public static readonly Vector2 PLAYER_SIZE = new Vector2(3, 1);

        /// <summary>
        /// 玩家的初始位置[Player0,Player1]
        /// </summary>
        public static readonly Vector2[] PLAYER_INITIAL_POSITION = new Vector2[] { new Vector2(0, -13), new Vector2(0, 13) };
    }
}
