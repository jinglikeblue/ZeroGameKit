using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using Zero;

namespace Sokoban
{
    class TileUtil
    {
        /// <summary>
        /// 格子转换为坐标
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public static Vector3 Tile2Position(Vector2Int tile)
        {
            return new Vector3(tile.x * Define.TILE_SIZE, tile.y * Define.TILE_SIZE);
        }

        /// <summary>
        /// 坐标转换为格子
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Vector2Int Position2Tile(Vector3 position)
        {
            return new Vector2Int((int)(position.x / Define.TILE_SIZE), (int)(position.y / Define.TILE_SIZE));
        }
    }
}
