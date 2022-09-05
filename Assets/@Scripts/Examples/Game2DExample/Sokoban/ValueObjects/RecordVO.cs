using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using ZeroHot;

namespace Sokoban
{
    class RecordVO
    {
        /// <summary>
        /// 箱子移动的方向
        /// </summary>
        public EDir dir;

        /// <summary>
        /// 角色所在位置
        /// </summary>
        public Vector2Int roleTile;

        /// <summary>
        /// 箱子所在位置(推动前）
        /// </summary>
        public Vector2Int boxTile;

        /// <summary>
        /// 箱子的索引
        /// </summary>
        public int boxIdx;

        public RecordVO(Vector2Int roleTile, Vector2Int boxTile, EDir dir, int boxIdx)
        {
            this.dir = dir;
            this.roleTile = roleTile;
            this.boxTile = boxTile;
            this.boxIdx = boxIdx;
        }
    }
}
