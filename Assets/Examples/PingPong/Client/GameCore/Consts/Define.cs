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
        /// 游戏赢家的临时数据KEY
        /// </summary>
        public const string WINNER_STORAGE_KEY = "winner";

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
        /// 球的起始移动速度（Y轴）
        /// </summary>
        public static readonly Number BALL_MOVE_SPEED = new Number(10);

        /// <summary>
        /// 球每秒增加的速度（Y轴）
        /// </summary>
        public static readonly Number BALL_ACCELERATED_SPEED = new Number(1, 10);

        /// <summary>
        /// 玩家的大小（宽，长）
        /// </summary>
        public static readonly Vector2 PLAYER_SIZE = new Vector2(3, 1);

        /// <summary>
        /// 玩家的初始位置[Player0,Player1]
        /// </summary>
        public static readonly Vector2[] PLAYER_INITIAL_POSITION = new Vector2[] { new Vector2(0, -13), new Vector2(0, 13) };

        /// <summary>
        /// 玩家的移动速度(X轴)
        /// </summary>
        public static readonly Number PLAYER_MOVE_SPEED = new Number(15);
    }
}
