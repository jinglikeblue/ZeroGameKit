﻿namespace Zero
{
    /// <summary>
    /// 资源版本号数据
    /// </summary>
    public class ResVerVO
    {
        /// <summary>
        /// 资源项
        /// </summary>
        public class Item
        {
            /// <summary>
            /// 资源名称
            /// </summary>
            public string name;

            /// <summary>
            /// 版本号
            /// </summary>
            public string version;

            /// <summary>
            /// 文件大小(字节为单位)
            /// </summary>
            public long size;
        }

        /// <summary>
        /// 标识符
        /// </summary>
        public string identifier = string.Empty;
        
        /// <summary>
        /// 资源数组
        /// </summary>
        public Item[] items;
    }
}