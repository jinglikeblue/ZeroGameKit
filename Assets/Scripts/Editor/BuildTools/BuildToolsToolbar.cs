using Jing;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using ZeroEditor;

namespace ZeroEditor
{
    public class BuildToolsToolbar
    {
        [MenuItem("构建/构建配置", false, 0)]
        public static void BuildToolsSetting()
        {
            BuildToolsEditorWin.Open();
        }

        /// <summary>
        /// 获取命令行携带的参数(搭配Jenkins使用）
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetCommandLineArgs()
        {
            Dictionary<string, string> argsDic = new Dictionary<string, string>();

            // 解析命令行参数
            string[] args = System.Environment.GetCommandLineArgs();
            foreach (var s in args)
            {
                if (s.Contains("--"))
                {
                    var kv = s.Substring(2).Split(':');
                    argsDic[kv[0]] = kv[1];
                    Debug.Log($"命令行参数： k = {kv[0]}  v = {kv[1]}");
                }
            }

            return argsDic;
        }

        [MenuItem("构建/构建当前平台")]
        public static void BuildCurrentPlatform()
        {
            Debug.Log($"是否是批处理命令模式： {Application.isBatchMode}");
            GetCommandLineArgs();

            switch (ZeroEditorConst.BUILD_PLATFORM)
            {
                case BuildTarget.StandaloneWindows:
                    BuildWin64();
                    break;
                case BuildTarget.StandaloneOSX:
                    break;
                case BuildTarget.Android:
                    BuildAPK();
                    break;
                case BuildTarget.iOS:
                    BuildXCodeProject();
                    break;
            }
        }

        [MenuItem("构建/测试")]
        public static void Test()
        {
            if (UnityEditor.EditorUtility.DisplayDialog("警告！", "测试时会依次对所有的平台进行构建，非常耗时！", "继续", "取消"))
            {
                BuildAPK();
                BuildXCodeProject();
                BuildWin64();
                BuildOSX();
            }

        }

        [MenuItem("构建/Android/Build APK")]
        public static void BuildAPK()
        {
            BuildPlayerOptions options = new BuildPlayerOptions();
            options.locationPathName = "ZeroGameKit.apk";
            options.target = BuildTarget.Android;
            options.targetGroup = BuildTargetGroup.Android;
            EditorUserBuildSettings.buildAppBundle = false;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
            PlayerSettings.keyaliasPass = "123456";
            PlayerSettings.keystorePass = "123456";
            Build(options);
        }

        [MenuItem("构建/Android/Build AAB")]
        public static void BuildAAB()
        {
            BuildPlayerOptions options = new BuildPlayerOptions();
            options.locationPathName = "ZeroGameKit.aab";
            options.target = BuildTarget.Android;
            options.targetGroup = BuildTargetGroup.Android;
            EditorUserBuildSettings.androidBuildType = AndroidBuildType.Release;
            EditorUserBuildSettings.buildAppBundle = true;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
            PlayerSettings.keyaliasPass = "123456";
            PlayerSettings.keystorePass = "123456";
            Build(options);
        }

        [MenuItem("构建/Android/Build Project")]
        public static void BuildAndroidProject()
        {
            BuildPlayerOptions options = new BuildPlayerOptions();
            options.locationPathName = "ZeroGameKit_Android";
            options.target = BuildTarget.Android;
            options.targetGroup = BuildTargetGroup.Android;
            EditorUserBuildSettings.buildAppBundle = false;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = true;            
            PlayerSettings.keyaliasPass = "123456";
            PlayerSettings.keystorePass = "123456";
            Build(options);
        }

        [MenuItem("构建/iOS/Build XCode Project")]
        public static void BuildXCodeProject()
        {
            BuildPlayerOptions options = new BuildPlayerOptions();
            options.locationPathName = "ZeroGameKit_XCode";
            options.target = BuildTarget.iOS;
            options.targetGroup = BuildTargetGroup.iOS;
            Build(options);
        }

        [MenuItem("构建/Win/Build Win64")]
        public static void BuildWin64()
        {
            BuildPlayerOptions options = new BuildPlayerOptions();
            options.locationPathName = "ZeroGameKit_Win/ZeroGameKit.exe";
            options.target = BuildTarget.StandaloneWindows;
            options.targetGroup = BuildTargetGroup.Standalone;
            Build(options);
        }

        [MenuItem("构建/Mac/Build OSX")]
        public static void BuildOSX()
        {
            BuildPlayerOptions options = new BuildPlayerOptions();
            options.locationPathName = "ZeroGameKit_OSX";
            options.target = BuildTarget.StandaloneOSX;
            options.targetGroup = BuildTargetGroup.Standalone;
            Build(options);
        }

        public static void Build(BuildPlayerOptions options)
        {
            options.locationPathName = FileUtility.CombinePaths("Bin", options.locationPathName);

            BuildPipeline.BuildPlayer(options);

            ZeroEditorUtility.OpenDirectory(Directory.GetParent(options.locationPathName).FullName);
        }
    }
}