using Jing;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ZeroEditor
{
    public class ZeroEditorUtility : EditorWindow
    {
        /// <summary>
        /// 打开目录
        /// </summary>
        /// <param name="path"></param>
        public static void OpenDirectory(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            var fullPath = FileUtility.CombineDirs(true, ZeroEditorConst.PROJECT_PATH, path);
            if (!Directory.Exists(fullPath))
            {
                Debug.LogError("[无法打开文件夹]不存在: " + fullPath);
                return;
            }

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                fullPath = FileUtility.StandardizeSlashSeparator(fullPath);
                System.Diagnostics.Process.Start("explorer.exe", fullPath);
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                var args = string.Format("{0} {1}", "Tools/Mac/OpenDir.sh", fullPath);
                System.Diagnostics.Process.Start("bash", args);
            }
        }

        /// <summary>
        /// 显示编辑器Tip信息.
        /// </summary>
        /// <param name="content"></param>
        [Obsolete("建议使用EditorWindowExtensions.ShowTip扩展方法")]
        public static void ShowTip(EditorWindow editorWin, string content)
        {
            editorWin.ShowTip(content);
        }

        /// <summary>
        /// 获取全部选中物体的路径 
        /// </summary>
        /// <param name="isAbsolutePath">是否获取的是绝对路径</param>
        /// <returns></returns>
        public static string[] GetSelectedObjectPathList(bool isAbsolutePath = false)
        {
            string[] paths = new string[Selection.objects.Length];
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                paths[i] = GetAssetAbsolutePath(AssetDatabase.GetAssetPath(Selection.objects[i]));
            }

            return paths;
        }

        /// <summary>  
        /// 获取资源的绝对路径  
        /// </summary>  
        /// <param name="path">Assets/Editor/...</param>  
        /// <returns></returns>  
        public static string GetAssetAbsolutePath(string assetPath)
        {
            string m_path = Application.dataPath;
            m_path = m_path.Substring(0, m_path.Length - 6);
            m_path += assetPath;
            return m_path;
        }

        /// <summary>
        /// 设置选中项为路径指向的资源
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool SetPathToSelection(string path)
        {
            var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            if (obj != null)
            {
                Selection.objects = new UnityEngine.Object[] { obj };
                return true;
            }

            return false;
        }

        /// <summary>
        /// 为当前项目创建一个分身项目。Assets等资源会链接到到当前项目的目录，这样代码可以统一管理。
        /// </summary>
        /// <param name="sourceProject">源项目</param>
        /// <param name="targetDir">创建分身的目标文件夹</param>
        /// <param name="extraFolders">除了默认要映射的文件夹之外，其它要映射的自定义文件夹</param>
        public static void CreateShadowProject(string sourceProject, string targetDir, string[] extraFolders = null)
        {
            if (false == Directory.Exists(targetDir))
            {
                //文件夹不存在，则创建
                Directory.CreateDirectory(targetDir);
            }
            else
            {
                //检查文件夹是否空的
                if (Directory.GetFiles(targetDir).Length > 0 || Directory.GetDirectories(targetDir).Length > 0)
                {
                    EditorUtility.DisplayDialog("错误提示", "目标文件夹有数据！需要选择一个新创建的文件夹！", "确定");
                    return;
                }
            }
            
            //使用cmd命令创建文件夹的映射
            string[] toLinkFolders = new string[]
            {
                "Assets",
                "Packages",
                "ProjectSettings"
            };

            var toLinkFolderList = new List<string>(toLinkFolders);
            
            if (null != extraFolders)
            {
                toLinkFolderList.AddRange(extraFolders);
            }
            
            foreach (var dir in toLinkFolderList)
            {
                var sourceFolder = FileUtility.CombineDirs(false,sourceProject, dir);
                var linkFolder = FileUtility.CombineDirs(false, targetDir, dir);
                #if UNITY_EDITOR_WIN
                sourceFolder = FileUtility.StandardizeSlashSeparator(sourceFolder);
                linkFolder = FileUtility.StandardizeSlashSeparator(linkFolder);
                #endif
                var cmdContent = $"mklink /d \"{linkFolder}\" \"{sourceFolder}\"";
                // Debug.Log(cmdContent);
                ProcessUtility.RunCommandLine(cmdContent);
            }
        }
    }
}