using System;

namespace Zero
{
    /// <summary>
    /// @Configs自动配置标签，添加该标记的对象会显示在 「Zero -> 配置文件编辑」中
    /// </summary>    
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class ZeroConfigAttribute : Attribute
    {
        /// <summary>
        /// 标签的名称
        /// </summary>
        public string label { get; private set; }

        /// <summary>
        /// 配置存储的位置，相对于@ab目录
        /// </summary>
        public string assetPath { get; private set; }

        public ZeroConfigAttribute(string label, string assetPath)
        {            
            this.label = label;
            this.assetPath = assetPath;
        }
    }
}