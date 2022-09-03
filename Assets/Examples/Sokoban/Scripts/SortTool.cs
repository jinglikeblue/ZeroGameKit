using System;
using System.Collections.Generic;

namespace Sokoban
{
    /// <summary>
    /// 对对象进行排序的工具
    /// </summary>
    public class SortTool<T>
    {
        struct SortItemVO<T>
        {
            public SortItemVO(int id, T obj)
            {
                this.sortId = id;
                this.obj = obj;
            }
            public readonly int sortId;
            public readonly T obj;
        }

        List<SortItemVO<T>> _itemList = new List<SortItemVO<T>>();

        /// <summary>
        /// 添加排序对象
        /// </summary>
        /// <param name="sortId">参与排序的值</param>
        /// <param name="obj">排序的对象</param>
        public void AddItem(int sortId, T obj)
        {
            SortItemVO<T> vo = new SortItemVO<T>(sortId, obj);
            _itemList.Add(vo);
        }

        /// <summary>
        /// 排序并获取排序后的结果
        /// </summary>
        /// <returns></returns>
        public T[] Sort(bool isDesc = false)
        {
            SortItemVO<T>[] arr = _itemList.ToArray();
            SortItemVO<T> temp;
            for (int i = 0; i < arr.Length - 1; i++)
            {
                for (int j = 0; j < arr.Length - 1 - i; j++)
                {
                    if (arr[j].sortId > arr[j + 1].sortId)
                    {
                        temp = arr[j + 1];
                        arr[j + 1] = arr[j];
                        arr[j] = temp;
                    }                    
                }
            }            

            T[] result = new T[arr.Length];            
            for (int i = 0; i < result.Length; i++)
            {
                if (isDesc)
                {
                    result[result.Length - i - 1] = arr[i].obj;
                }
                else
                {
                    result[i] = arr[i].obj;
                }
            }
            return result;
        }

        int Compare(SortItemVO<T> a, SortItemVO<T> b)
        {
            if(a.sortId > b.sortId)
            {
                return 1;
            }
            return -1;
        }
    }
}
