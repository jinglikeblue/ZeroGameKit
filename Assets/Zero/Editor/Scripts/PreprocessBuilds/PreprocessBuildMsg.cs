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

            HybridCLRSettings.Instance.enable = isHybridClrEnable;
            HybridCLRSettings.Save();
            Debug.Log(LogColor.Zero2($"[Zero][Build][PreprocessBuild] HybridCLR是否启用:{isHybridClrEnable}"));
        }
    }
}