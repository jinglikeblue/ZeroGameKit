using Jing;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
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
        /// 在IDE中编辑脚本
        /// </summary>
        /// <param name="path"></param>
        public static void EditScript(string path)
        {
            InternalEditorUtility.OpenFileAtLineExternal(path, 0, 0);
        }

        /// <summary>
        /// 在IDE中编辑脚本
        /// </summary>
        /// <param name="obj"></param>
        public static void EditScript(object obj)
        {
            if (null == obj) return;

            try
            {
                var classType = obj.GetType();
                var isSuccess = FindAsFileName(classType);
                if (false == isSuccess)
                {
                    isSuccess = FindInAllScripts(classType);
                }

                if (false == isSuccess)
                {
                    Debug.LogError($"无法找到对应的脚本文件: {classType.Name}");
                }

                // 打开该脚本文件
                // UnityEditor.MonoScript script = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.MonoScript>(assetPath);
                // UnityEditor.AssetDatabase.OpenAsset(script);
            }
            catch (Exception e)
            {
                Debug.LogError($"无法打开脚本文件: {obj.GetType().FullName}");
                Debug.LogError(e);
            }

            bool FindAsFileName(Type classType)
            {
                // 在项目中查找类名对应的脚本文件
                string[] guids = UnityEditor.AssetDatabase.FindAssets($"{classType.Name} t:Script");

                if (0 == guids.Length)
                {
                    return false;
                }

                // 获取脚本的路径
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                var line = FindClassLineNumber(assetPath, classType.Name);
                if (-1 == line)
                {
                    return false;
                }

                InternalEditorUtility.OpenFileAtLineExternal(assetPath, line, 0);
                return true;
            }

            bool FindInAllScripts(Type classType)
            {
                var files = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);
                foreach (var t in files)
                {
                    var filePath = FileUtility.StandardizeBackslashSeparator(t);
                    if (false == CheckClassNamespace(filePath, classType.Namespace))
                    {
                        continue;
                    }

                    var line = FindClassLineNumber(filePath, classType.Name);
                    if (line > -1)
                    {
                        InternalEditorUtility.OpenFileAtLineExternal(filePath, line + 1, 0);
                        return true;
                    }
                }

                return false;
            }

            bool CheckClassNamespace(string filePath, string namespaceName)
            {
                if (File.ReadAllText(filePath).Contains($"namespace {namespaceName}"))
                {
                    return true;
                }

                return false;
            }

            int FindClassLineNumber(string filePath, string className)
            {
                var lines = File.ReadAllLines(filePath);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains($"class {className}"))
                    {
                        return i;
                    }
                }

                return -1;
            }
        }
        
        /// <summary>
        /// 是否设置的使用IL2CPP进行代码编译
        /// </summary>
        public static bool IsScriptingBackendIL2CPP => PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup) == ScriptingImplementation.IL2CPP;

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
                var sourceFolder = FileUtility.CombineDirs(false, sourceProject, dir);
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