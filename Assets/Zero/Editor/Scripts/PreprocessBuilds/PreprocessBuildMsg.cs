using System.IO;
using HybridCLR.Editor.Settings;
using Jing;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEditor;
using Zero;

namespace ZeroEditor
{
    /// <summary>
    /// Build前的预处理内容
    /// </summary>
    public class PreprocessBuildMsg : IPreprocessBuildWithReport
    {
        public int callbackOrder => int.MinValue;

        /// <summary>
        /// 是否是构建WebGL目标平台
        /// </summary>
        private static bool IsBuildWebGL => ZeroEditorConst.BUILD_PLATFORM == BuildTarget.WebGL;

        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log(LogColor.Zero2("[Zero][Build][PreprocessBuild] Build前处理"));

            //[待观察]检查输出路径，来判断是否是HybridCLR的构建
            if (CheckIsHybridClrGenerate(report))
            {
                Debug.Log(LogColor.Zero1("[Zero][Build][PreprocessBuild] 检查到在进行HybridCLR部署构建。"));
                if (false == HybridCLRSettings.Instance.enable)
                {
                    Debug.Log(LogColor.Zero1("[Zero][Build][PreprocessBuild] HybridCLR功能未开启，为了构建成功暂时开启HybridCLR功能，不影响Build安装包时的判断。"));
                    HybridCLRSettings.Instance.enable = true;
                    HybridCLRSettings.Save();
                }
            }
            else
            {
                SyncHybridClrEnable();
                BuildInfo.GenerateBuildInfo();
            }

            PreprocessForWebGL();
        }

        private static bool CheckIsHybridClrGenerate(BuildReport report)
        {
            var outputPath = FileUtility.StandardizeBackslashSeparator(report.summary.outputPath);
            if (outputPath.Contains("HybridCLRData/StrippedAOTDllsTempProj/"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 同步HybridCLR功能是否开启的设置
        /// </summary>
        private static void SyncHybridClrEnable()
        {
            //如果启动了dll，且打包方式为IL2CPP，则启用HybridCLR
            bool isHybridClrEnable = false;

            var setting = LauncherSetting.LoadLauncherSettingDataFromResources();
            if (setting.isUseDll && ZeroEditorUtility.IsScriptingBackendIL2CPP)
            {
                isHybridClrEnable = true;
            }

            //目标平台是WegGL时，强制关闭HybridCLR
            if (IsBuildWebGL)
            {
                HybridCLREditorUtility.CleanGeneratedFiles(false);
                isHybridClrEnable = false;
            }

            HybridCLRSettings.Instance.enable = isHybridClrEnable;
            HybridCLRSettings.Save();
            Debug.Log(LogColor.Zero2($"[Zero][Build][PreprocessBuild] HybridCLR是否启用:{isHybridClrEnable}"));
        }

        /// <summary>
        /// 针对WebGL平台的预处理检查
        /// </summary>
        private static void PreprocessForWebGL()
        {
            if (!IsBuildWebGL)
            {
                return;
            }

            Debug.Log(LogColor.Zero1("[Zero][Build][PreprocessBuild][WebGL] Build前处理"));
            //检查设置Gzip以保证构建的资源足够小
            if (PlayerSettings.WebGL.compressionFormat == WebGLCompressionFormat.Disabled)
            {
                Debug.Log(LogColor.Yellow("[Zero][Build][PreprocessBuild][WebGL] 未设置压缩格式，会导致构建的资源量太大，建议设置为Gzip!"));
            }

            if (PlayerSettings.WebGL.compressionFormat == WebGLCompressionFormat.Gzip && !PlayerSettings.WebGL.decompressionFallback)
            {
                Debug.Log(LogColor.Yellow("[Zero][Build][PreprocessBuild][WebGL] 使用Gzip压缩时，如果未勾选(Decompression Fallback)，可能会导致无法正常启动游戏!"));
            }
        }
    }
}