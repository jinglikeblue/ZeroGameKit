using Jing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zero;

namespace ZeroEditor
{
    public interface IOptimizeSettingVO
    {
        string GetFolder();
    }

    /// <summary>
    /// 优化配置模型
    /// </summary>
    class OptimizeSettingModel<T> where T : IOptimizeSettingVO
    {
        PathTree<T> _pathTree = new PathTree<T>();

        string[] SplitFolderPath(string folder)
        {
            var path = FileUtility.StandardizeBackslashSeparator(folder);
            path = FileUtility.RemoveStartPathSeparator(path);
            var paths = path.Split('/');
            return paths;
        }

        /// <summary>
        /// 整理纹理优化配置，方便查找
        /// </summary>
        /// <param name="textureSettings"></param>
        public void TidySettings(List<T> settings)
        {
            _pathTree.Clear();
            foreach (var setting in settings)
            {
                var paths = SplitFolderPath(setting.GetFolder());
                _pathTree.Create(paths).data = setting;
            }

            var list = _pathTree.SearchNodes((node) => { return node.data == null ? false : true; });

            // if (list.Count > 0)
            // {
            //     StringBuilder sb = new StringBuilder();
            //     sb.AppendLine("[");
            //     foreach (var node in list)
            //     {
            //         sb.AppendLine($"    {FileUtility.RemoveStartPathSeparator(node.ToPathString())}");
            //     }
            //
            //     sb.AppendLine("]");
            //     Debug.Log(sb.ToString());
            // }
        }

        /// <summary>
        /// 找到和路径匹配的配置
        /// </summary>
        /// <param name="path"></param>
        public T FindSetting(string path)
        {
            var paths = SplitFolderPath(path);
            var node = _pathTree.Find(paths, false);
            if (null == node)
            {
                return default(T);
            }

            var setting = PathTree<T>.FindLastNodeWithNonNullDataForward(node);
            if (null == setting)
            {
                return default(T);
            }

            return setting.data;
        }
    }
}