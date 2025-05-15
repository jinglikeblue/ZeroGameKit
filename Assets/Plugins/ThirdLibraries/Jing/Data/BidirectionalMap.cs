using System;
using System.Collections.Generic;
using System.Linq;

namespace Jing
{
    /// <summary>
    /// 双向映射表。设置的两个值，可以通过其中任意一个查找到另一个
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    public class BidirectionalMap<TLeft, TRight>
    {
        /// <summary>
        /// 映射关系项
        /// </summary>
        public struct MappingItem
        {
            public TLeft left;
            public TRight right;
        }

        Dictionary<TLeft, TRight> _leftToRightMap = new Dictionary<TLeft, TRight>();
        Dictionary<TRight, TLeft> _rightToLeftMap = new Dictionary<TRight, TLeft>();

        /// <summary>
        /// 获取所有左侧的数据
        /// </summary>
        /// <returns></returns>
        public TLeft[] GetLefts()
        {
            return _leftToRightMap.Keys.ToArray();
        }

        /// <summary>
        /// 获取所有右侧的数据
        /// </summary>
        /// <returns></returns>
        public TRight[] GetRights()
        {
            return _rightToLeftMap.Keys.ToArray();
        }
        
        /// <summary>
        /// 设置一个双向映射关系
        /// </summary>
        public void Set(TLeft l, TRight r)
        {
            if (_leftToRightMap.ContainsKey(l))
            {
                throw new Exception($"已设置了映射关系 (Left)[{l}] <=> (Right)[{_leftToRightMap[l]}]");
            }

            if (_rightToLeftMap.ContainsKey(r))
            {
                throw new Exception($"已设置了映射关系 (Left)[{_rightToLeftMap[r]}] <=> (Right)[{r}]");
            }

            _leftToRightMap[l] = r;
            _rightToLeftMap[r] = l;
        }

        /// <summary>
        /// 通过左方值获取右方值
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public TRight GetRight(TLeft l)
        {
            if (_leftToRightMap.ContainsKey(l))
            {
                return _leftToRightMap[l];
            }

            return default(TRight);
        }

        /// <summary>
        /// 通过右方值获取左方值
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public TLeft GetLeft(TRight r)
        {
            if (_rightToLeftMap.ContainsKey(r))
            {
                return _rightToLeftMap[r];
            }

            return default(TLeft);
        }

        /// <summary>
        /// 检查映射关系是否存在
        /// </summary>
        /// <param name="l"></param>
        public bool ContainsLeft(TLeft l)
        {
            return _leftToRightMap.ContainsKey(l);
        }

        /// <summary>
        /// 检查映射关系是否存在
        /// </summary>
        /// <param name="r"></param>
        public bool ContainsRight(TRight r)
        {
            return _rightToLeftMap.ContainsKey(r);
        }

        /// <summary>
        /// 获取所有的映射关系
        /// </summary>
        /// <returns>分别是左边的数据列表以及右边的数据列表</returns>
        public MappingItem[] GetMappings()
        {
            MappingItem[] items = new MappingItem[_leftToRightMap.Count];
            var i = 0;
            foreach (var kv in _leftToRightMap)
            {
                items[i] = new MappingItem()
                {
                    left = kv.Key,
                    right = kv.Value
                };
            }
            return items;
        }
    }
}
