using System.Collections.Generic;
using Jing;
using UnityEngine;

namespace ZeroEditor
{
    /// <summary>
    /// Hierarchy的工具类
    /// </summary>
    public static class HierarchyEditorUtility
    {
        /// <summary>
        /// 获取节点的路径
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static string GetNodePath(GameObject gameObject)
        {
            List<string> list = new List<string>();
            while (gameObject != null)
            {
                list.Add(gameObject.name);
                if (gameObject.transform.parent != null)
                {
                    gameObject = gameObject.transform.parent.gameObject;
                }
                else
                {
                    gameObject = null;
                }
            }

            list.Reverse();

            var path = "";
            path = FileUtility.CombinePaths(list.ToArray());
            return path;
        }
    }
}