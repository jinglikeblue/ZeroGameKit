using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using Zero;

namespace Sokoban
{
    /// <summary>
    /// 方向
    /// </summary>
    public enum EDir
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        NONE
    }

    /// <summary>
    /// 单位的类型
    /// </summary>
    public enum EUnitType
    {
        ROLE,
        BLOCK,
        BOX,
        TARGET
    }

    class Define
    {
        /// <summary>
        /// 关卡总数
        /// </summary>
        public const int LEVEL_AMOUNT = 97;

        /// <summary>
        /// 格子大小
        /// </summary>
        public const float TILE_SIZE = 0.48f;

        /// <summary>
        /// 地图大小（MAP_TILE_COUNT_OF_SIDE * MAP_TILE_COUNT_OF_SIDE 格子数）
        /// </summary>
        public const int MAP_TILE_COUNT_OF_SIDE = 16;
    }
}
