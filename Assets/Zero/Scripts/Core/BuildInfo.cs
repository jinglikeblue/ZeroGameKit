using System.IO;
using System.Text;
using Jing;
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
        public const string FileName = "zero_build_info.txt";

        private const string FlagIL2CPP = "IL2CPP";
        private const string FlagHybridClr = "HybridCLR";

        /// <summary>
        /// 是否是IL2CPP编译模式
        /// </summary>
        public bool IsIL2CPP { get; private set; }

        /// <summary>
        /// 是否开启了HybridCLR
        /// </summary>
        public bool IsHybridClrEnable { get; private set; }

        public static BuildInfo TryLoadBuildInfo()
        {
#if UNITY_EDITOR
            return CreateBuildInfo();
#endif

            var text = StreamingAssetsUtility.LoadText(FileName);
            if (null == text)
            {
                Debug.LogError($"[Zero][BuildInfo] 文件不存在:{FileName}");
                return new BuildInfo();
            }

            BuildInfo buildInfo = new BuildInfo();
            try
            {
                using (StringReader reader = new StringReader(text))
                {
                    string line;
                    while ((line = reader.ReadLine()?.Trim()) != null)
                    {
                        switch (line)
                        {
                            case FlagHybridClr:
                                buildInfo.IsHybridClrEnable = true;
                                break;
                            case FlagIL2CPP:
                                buildInfo.IsIL2CPP = true;
                                break;
                        }
                    }
                }
            }
            catch
            {
                buildInfo = new BuildInfo();
            }

            return buildInfo;
        }

#if UNITY_EDITOR

        private static BuildInfo CreateBuildInfo()
        {
            BuildInfo bi = new BuildInfo();
            bi.IsIL2CPP = UnityEditor.PlayerSettings.GetScriptingBackend(UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup) == UnityEditor.ScriptingImplementation.IL2CPP;
            bi.IsHybridClrEnable = HybridCLR.Editor.Settings.HybridCLRSettings.Instance.enable;
            return bi;
        }

        [UnityEditor.MenuItem("Test/BuildInfo/Generate")]
        public static void GenerateBuildInfo()
        {
            var bi = CreateBuildInfo();

            StringBuilder sb = new StringBuilder();

            if (bi.IsIL2CPP)
            {
                sb.AppendLine(FlagIL2CPP);
            }

            if (bi.IsHybridClrEnable)
            {
                sb.AppendLine(FlagHybridClr);
            }

            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }

            var path = FileUtility.CombinePaths(Application.streamingAssetsPath, FileName);
            File.WriteAllText(path, sb.ToString().Trim());
            Debug.Log(LogColor.Zero2($"[Zero][BuildInfo] 构建BuildInfo: {path}"));
            UnityEditor.AssetDatabase.Refresh();
        }

        [UnityEditor.MenuItem("Test/BuildInfo/Load")]
        private static void TestLoadBuildInfo()
        {
            Debug.Log(Json.ToJsonIndented(TryLoadBuildInfo()));
        }
#endif
    }
}