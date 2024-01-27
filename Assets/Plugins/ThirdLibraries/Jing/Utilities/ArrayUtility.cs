﻿using System;
using System.Collections.Generic;

namespace Jing
{
    public class ArrayUtility
    {
        /// <summary>
        /// 对列表进行排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="sortFunction"></param>
        public static List<T> Sort<T>(List<T> list, Func<object, object, bool> sortFunction)
        {
            List<T> sorted = new List<T>(list.Count);
            sorted.Add(list[0]);
            for (int i = 1; i < list.Count; i++)
            {
                var toInsert = list[i];

                bool isInserted = false;
                for (int j = 0; j < sorted.Count; j++)
                {
                    var b = sorted[j];

                    if (sortFunction(toInsert, b))
                    {
                        isInserted = true;
                        sorted.Insert(j, toInsert);
                        break;
                    }
                }

                //如果没有插入过，则添加到末尾
                if (isInserted == false)
                {
                    sorted.Add(toInsert);
                }
            }
            return sorted;
        }

        /// <summary>
        /// 对数组进行排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="sortFunction"></param>
        /// <returns></returns>
        public static T[] Sort<T>(T[] list, Func<object, object, bool> sortFunction)
        {
            List<T> sorted = new List<T>(list.Length);
            sorted.Add(list[0]);
            for (int i = 1; i < list.Length; i++)
            {
                var toInsert = list[i];

                bool isInserted = false;
                for (int j = 0; j < sorted.Count; j++)
                {
                    var b = sorted[j];

                    if (sortFunction(toInsert, b))
                    {
                        isInserted = true;
                        sorted.Insert(j, toInsert);
                        break;
                    }
                }

                //如果没有插入过，则添加到末尾
                if (isInserted == false)
                {
                    sorted.Add(toInsert);
                }
            }
            return sorted.ToArray();
        }
    }
}
