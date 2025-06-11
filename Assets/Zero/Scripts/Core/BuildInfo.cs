using System.IO;
using Jing;
using Newtonsoft.Json;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 构建信息类
    /// 获取一些仅在Editor下才能获取到的，打包时保存下来的信息。
    /// </summary>
    public class BuildInfo
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public const string FileName = "zero_build_info.json";

        /// <summary>
        /// 是否是IL2CPP编译模式
        /// </summary>
        [JsonProperty]
        public bool IsIL2CPP { get; private set; }

        /// <summary>
        /// 是否开启了HybridCLR
        /// </summary>
        [JsonProperty]
        public bool IsHybridClrEnable { get; private set; }

        /// <summary>
        /// 平台名称
        /// </summary>
        [JsonProperty]
        public string PlatformName { get; private set; }

        public static BuildInfo TryLoadBuildInfo()
        {
#if UNITY_EDITOR
            GenerateBuildInfo();
#endif

            BuildInfo buildInfo = LoadBuildInfo();
            return buildInfo;
        }

        private static BuildInfo LoadBuildInfo()
        {
            var ta = Resources.Load<TextAsset>(Path.GetFileNameWithoutExtension(FileName));
            if (null == ta)
            {
                Debug.Log(LogColor.Red($"[Zero][BuildInfo] 未读取到BuildInfo文件: {FileName} "));
                return null;
            }

            var content = ta.text;
            var bi = Json.ToObject<BuildInfo>(content);
            return bi;
        }


#if UNITY_EDITOR

        private static BuildInfo CreateBuildInfo()
        {
            BuildInfo bi = new BuildInfo();
            bi.IsIL2CPP = UnityEditor.PlayerSettings.GetScriptingBackend(UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup) == UnityEditor.ScriptingImplementation.IL2CPP;
            bi.IsHybridClrEnable = HybridCLR.Editor.Settings.HybridCLRSettings.Instance.enable;
            bi.PlatformName = ZeroConst.PLATFORM_DIR_NAME;
            return bi;
        }

        [UnityEditor.MenuItem("Test/BuildInfo/Generate")]
        public static void GenerateBuildInfo()
        {
            var bi = CreateBuildInfo();
            var content = Json.ToJsonIndented(bi);

            var folder = FileUtility.CombinePaths(ZeroConst.PROJECT_ASSETS_DIR, "Resources");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var path = FileUtility.CombinePaths(folder, FileName);
            File.WriteAllText(path, content);
            Debug.Log(LogColor.Zero2($"[Zero][BuildInfo] 构建BuildInfo: {path}"));
            UnityEditor.AssetDatabase.Refresh();
        }

        [UnityEditor.MenuItem("Test/BuildInfo/Load")]
        private static void TestLoadBuildInfo()
        {
            var bi = TryLoadBuildInfo();
            Debug.Log(Json.ToJsonIndented(bi));
        }
#endif
    }
}