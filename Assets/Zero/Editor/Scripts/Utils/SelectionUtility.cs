using UnityEngine;

namespace ZeroEditor
{
    /// <summary>
    /// 选取器工具类
    /// </summary>
    public static class SelectionUtility
    {
        /// <summary>
        /// 选中guid对应的资源
        /// </summary>
        /// <param name="guid"></param>
        public static void SelectAssetByGuid(string guid)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            if (!string.IsNullOrEmpty(path))
            {
                UnityEngine.Object asset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                if (asset != null)
                {
                    // UnityEditor.Selection.activeObject = asset; // 选中单个资源
                    UnityEditor.EditorGUIUtility.PingObject(asset); // 在 Project 窗口高亮显示
                }
                else
                {
                    Debug.LogError("资源加载失败");
                }
            }
            else
            {
                Debug.LogError("无效的 GUID 或资源不存在");
            }
        }
    }
}