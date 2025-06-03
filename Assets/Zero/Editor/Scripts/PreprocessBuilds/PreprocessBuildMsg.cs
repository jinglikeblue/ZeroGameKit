using HybridCLR.Editor.Settings;
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
        public int callbackOrder
        {
            get { return int.MinValue; }
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log("[Zero][Build][PreprocessBuild] Build前处理");
            SyncHybridClrEnable();
            BuildInfo.GenerateBuildInfo();
        }

        /// <summary>
        /// 同步HybridCLR功能是否开启的设置
        /// </summary>
        private static void SyncHybridClrEnable()
        {
            //如果启动了dll，且打包方式为IL2CPP，则启用HybridCLR
            bool isHybridClrEnable = false;
            var setting = LauncherSetting.LoadLauncherSettingDataFromResources();
            var scriptingBackend = PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (setting.isUseDll && scriptingBackend == ScriptingImplementation.IL2CPP)
            {
                isHybridClrEnable = true;
            }
            
            HybridCLRSettings.Instance.enable = isHybridClrEnable;
            HybridCLRSettings.Save();
            Debug.Log($"[Zero][Build][PreprocessBuild] HybridCLR是否启用:{isHybridClrEnable}");
        }
    }
}