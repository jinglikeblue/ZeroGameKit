using System;
using System.IO;
using Jing;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Zero;

namespace ZeroEditor
{
    /// <summary>
    /// Build后的处理内容
    /// </summary>
    public class PostprocessBuildMsg : IPostprocessBuildWithReport
    {
        public int callbackOrder => int.MaxValue;

        /// <summary>
        /// 是否是构建WebGL目标平台
        /// </summary>
        private static bool IsBuildWebGL => ZeroEditorConst.BUILD_PLATFORM == UnityEditor.BuildTarget.WebGL;

        public void OnPostprocessBuild(BuildReport report)
        {
            Debug.Log(LogColor.Zero2("[Zero][Build][PostprocessBuild] Build后处理"));

            try
            {
                // 展示Console面板，方便看日志
                var consoleWindowType = System.Type.GetType("UnityEditor.ConsoleWindow,UnityEditor");
                UnityEditor.EditorWindow.GetWindow(consoleWindowType).Focus();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            // 获取构建路径
            string buildPath = report.summary.outputPath;
            Debug.Log($"[Zero][Build][PostprocessBuild] 构建路径: {buildPath}");

            PostprocessForWebGL(report);
        }

        /// <summary>
        /// WebGL平台后处理
        /// </summary>
        public static void PostprocessForWebGL(BuildReport report)
        {
            if (!IsBuildWebGL)
            {
                return;
            }

            // Debug.Log(LogColor.Zero1("[Zero][Build][PostprocessBuild][WebGL] Build后处理"));

            try
            {
                var di = new DirectoryInfo(FileUtility.CombinePaths(report.summary.outputPath, "Build"));
                long size = 0;
                // 计算当前目录下的所有文件大小
                foreach (FileInfo file in di.GetFiles("*", SearchOption.AllDirectories))
                {
                    size += file.Length;
                }

                // 换算为MB
                var sizeMB = (size / 1000) / 1000f;
                Debug.Log($"[Zero][Build][PostprocessBuild][WebGL] Build目录大小: {sizeMB} MB");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}