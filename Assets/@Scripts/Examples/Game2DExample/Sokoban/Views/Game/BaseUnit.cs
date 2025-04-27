using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using Zero;

namespace Sokoban
{
    class BaseUnit : AView
    {
        /// <summary>
        /// 关联的图片
        /// </summary>
        protected SpriteRenderer Img { get; private set; }

        /// <summary>
        /// 单位类型
        /// </summary>
        public EUnitType UnitType { get; private set; }

        /// <summary>
        /// 所在格子
        /// </summary>
        public Vector2Int Tile { get; protected set; }

        /// <summary>
        /// 渲染图片
        /// </summary>
        SpriteRenderer _sr;

        protected override void OnInit(object data)
        {
            Img = GetChildComponent<SpriteRenderer>(0);
            _sr = GetChildComponent<SpriteRenderer>(0);

            UnitType = (EUnitType)data;
        }

        /// <summary>
        /// 设置所在的格子
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public virtual void SetTile(ushort x, ushort y)
        {
            Tile = new Vector2Int(x, y);
            gameObject.transform.localPosition = new Vector3(Tile.x * Define.TILE_SIZE, Tile.y * Define.TILE_SIZE);
        }

        /// <summary>
        /// 得到排序值
        /// </summary>
        /// <returns></returns>
        public float SortValue
        {
            get
            {
                return gameObject.transform.localPosition.y;
            }
        }

        /// <summary>
        /// 设置排序值
        /// </summary>
        /// <param name="v"></param>
        public void SetSortValue(int v)
        {
            if (null == _sr)
            {
                Debug.Log(gameObject.name);
            }
            _sr.sortingOrder = v;            
        }
    }
}
