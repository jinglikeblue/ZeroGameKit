﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Jing.TurbochargedScrollList
{
    /// <summary>
    /// 滚动列表接口
    /// </summary>
    public interface IScrollList
    {
        /// <summary>
        /// 移除指定索引位置的数据
        /// </summary>
        /// <param name="index"></param>
        void RemoveAt(int index);

        /// <summary>
        /// 清空列表，实际上是将数据清除，并将对象放入对象池
        /// </summary>
        void Clear();

        ScrollRect scrollRect { get; }

        /// <summary>
        /// 列表项数量
        /// </summary>
        int ItemCount { get; }

        /// <summary>
        /// 显示内容宽度
        /// </summary>
        float ContentWidth { get; }

        /// <summary>
        /// 显示内容高度
        /// </summary>
        float ContentHeight { get; }

        /// <summary>
        /// 检查指定的列表项是否在显示列表中
        /// </summary>
        /// <param name="index">列表项索引</param>
        /// <returns></returns>
        bool CheckItemShowing(int index);

        /// <summary>
        /// 滚动列表到列表项
        /// </summary>
        /// <param name="index">列表项索引</param>
        void ScrollToItem(int index);

        /// <summary>
        /// 滚动列表到指定位置
        /// </summary>
        /// <param name="position">目标像素位置</param>
        void ScrollToPosition(Vector2 position);

        /// <summary>
        /// 渲染列表项
        /// </summary>
        event RenderItemDelegate onRenderItem;

        /// <summary>
        /// 重构内容的事件
        /// </summary>
        event Action onRebuildContent;

        /// <summary>
        /// 刷新的事件
        /// </summary>
        event Action onRefresh;

        /// <summary>
        /// 当一个列表项被复用时触发
        /// </summary>
        event ItemBeforeReuseDelegate onItemBeforeReuse;

        /// <summary>
        /// 距离列表末尾的距离
        /// </summary>
        Vector2 GetDistanceToEnd();

        /// <summary>
        /// 添加一组数据
        /// </summary>
        /// <param name="collection"></param>
        void AddRange<TData>(IEnumerable<TData> collection);

        /// <summary>
        /// 添加一个数据
        /// </summary>
        /// <param name="data"></param>
        void Add(object data);

        /// <summary>
        /// 在指定位置，插入一个数据
        /// </summary>
        /// <param name="index"></param>
        /// <param name="data"></param>
        void Insert(int index, object data);

        /// <summary>
        /// 移除列表找到的第一个和data数据相同的项
        /// </summary>
        /// <param name="data"></param>
        /// <returns>有数据被移除，返回true</returns>
        bool Remove(object data);

        /// <summary>
        /// 获取数据拷贝
        /// </summary>
        T[] GetDatasCopy<T>();
    }
}
