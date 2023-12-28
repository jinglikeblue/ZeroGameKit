using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPong
{
    /// <summary>
    /// 配置数据
    /// </summary>
    public class ConfigVO
    {
        /// <summary>
        /// 世界宽度
        /// </summary>
        public int worldWidth;

        /// <summary>
        /// 世界高度
        /// </summary>
        public int worldHeight;

        /// <summary>
        /// 角色宽度
        /// </summary>
        public int playerWidth;

        /// <summary>
        /// 角色高度
        /// </summary>
        public int playerHeight;

        /// <summary>
        /// 玩家的站位。这里指的是Y轴上，玩家距离自己最近的底边的站位。
        /// </summary>
        public int playerPosition;

        /// <summary>
        /// 球在Y轴上的默认速度
        /// </summary>
        public float ballDefaultSpeedY;
    }
}
