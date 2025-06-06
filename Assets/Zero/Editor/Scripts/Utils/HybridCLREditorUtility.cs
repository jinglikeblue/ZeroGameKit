using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HybridCLR.Editor.Commands;
using HybridCLR.Editor.Installer;
using HybridCLR.Editor.Settings;
using Jing;
using UnityEditor;
using UnityEngine;
using Zero;

namespace ZeroEditor
{
    /// <summary>
    /// HybridCLR工具类
    /// </summary>
    public static class HybridCLREditorUtility
    {
        /// <summary>
        /// 是否在使用HybridCLR
        /// </summary>
        public static bool IsHybridCLRUsed
        {
            get
            {
                var vo = LauncherSetting.LoadLauncherSettingDataFromResources();
                if (vo.isUseDll)
                {
                    return true;
                }

                return false;
            }
        }
        
        /// <summary>
        /// 拷贝要进行补充元数据的dlls
        /// </summary>
        [MenuItem("Test/HybridCLR/CopyPatchedAOTAssemblyList")]
        public static void CopyPatchedAOTAssemblyList()
        {
            string outputStrippedDir = HybridCLR.Editor.SettingsUtil.GetAssembliesPostIl2CppStripDir(EditorUserBuildSettings.activeBuildTarget);
            string targetDir = FileUtility.CombineDirs(false, Application.dataPath, "HybridCLRGenerate", "Resources", HybridCLRAotMetadata.AOT_DLL_RESOURCES_DIR);

            if (Directory.Exists(targetDir)) Directory.Delete(targetDir, true);

            var toCopyAssemblies = AOTGenericReferences.PatchedAOTAssemblyList.ToArray();
            foreach (var assembly in toCopyAssemblies)
            {
                var sourcePath = FileUtility.CombinePaths(outputStrippedDir, assembly);

                var targetPath = FileUtility.CombinePaths(targetDir, assembly + ".bytes");

                FileUtility.CopyFile(sourcePath, targetPath, true);
                Debug.Log(LogColor.Zero2($"拷贝了元数据补充程序集: [{sourcePath}] => [{targetPath}]"));
            }
        }

        /// <summary>
        /// 一键准备HybridCLR
        /// </summary>
        public static async void OneClickForAll()
        {
            try
            {
                if (false == ZeroEditorUtility.IsScriptingBackendIL2CPP)
                {
                    EditorUtility.DisplayDialog("HybridCLR 部署出错", "当前代码编译环境并非IL2CPP，无法部署HybridCLR", "确定");
                    return;
                }
                
                if (false == EditorUtility.DisplayDialog("提示", "仅在Build工程之前需要执行！\n耗时较长，是否继续？", "继续", "取消"))
                {
                    return;
                }
                
                Debug.Log($"[Zero][HybridCLR] 开始一键部署");
                
                EditorUtility.DisplayProgressBar("HybridCLR 一键部署", "检测HybridCLR安装情况", 0f);
                Debug.Log($"[Zero][HybridCLR] 检测HybridCLR安装情况");
                AutoInstallHybridCLR();
                await Task.Delay(1);

                EditorUtility.DisplayProgressBar("HybridCLR 一键部署", "生成热更Dll，后续HybridCLR需要使用", 0.25f);
                Debug.Log($"[Zero][HybridCLR] 生成热更Dll，后续HybridCLR需要使用");
                await HotResEditorUtility.GenerateScriptAssembly();
                await Task.Delay(1);

                EditorUtility.DisplayProgressBar("HybridCLR 一键部署", "调用「HybridCLR/Generate/All」", 0.5f);
                Debug.Log($"[Zero][HybridCLR] 调用「HybridCLR/Generate/All」");
                PrebuildCommand.GenerateAll();
                await Task.Delay(1);

                EditorUtility.DisplayProgressBar("HybridCLR 一键部署", "拷贝元数据补充需要的程序集", 1f);
                Debug.Log($"[Zero][HybridCLR] 拷贝元数据补充需要的程序集");
                CopyPatchedAOTAssemblyList();
                await Task.Delay(1);

                Debug.Log($"[Zero][HybridCLR] 完成一键部署");
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                EditorUtility.DisplayDialog("HybridCLR 部署出错", "请检查HybridCLR 环境是否正确配置。", "确定");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        /// <summary>
        /// 自动安装HybridCLR。如果已安装，则不会重复安装。
        /// </summary>
        [MenuItem("Test/HybridCLR/AutoInstallHybridCLR")]
        public static void AutoInstallHybridCLR()
        {
            var controller = new InstallerController();

            if (!controller.HasInstalledHybridCLR())
            {
                Debug.Log(LogColor.Zero1($"没有检测到安装了HybridCLR，开始安装..."));
                controller.InstallDefaultHybridCLR();

                Debug.Log(LogColor.Zero1($"安装了HybridCLR，设置参数"));
                HybridCLRSettings.Instance.hotUpdateAssemblies = new[] { "scripts" };
                HybridCLRSettings.Instance.externalHotUpdateAssembliyDirs = new[] { "LibraryZero/ReleaseCache/dll" };
                HybridCLRSettings.Save();
            }
            else
            {
                Debug.Log(LogColor.Zero1($"检测到安装了HybridCLR"));
            }
        }

        /// <summary>
        /// 检查HybridCLR安装状态
        /// </summary>
        [MenuItem("Test/HybridCLR/CheckHybridCLRInstallState")]
        public static bool CheckHybridCLRInstallState()
        {
            var controller = new InstallerController();
            if (!controller.HasInstalledHybridCLR())
            {
                if (EditorUtility.DisplayDialog("提示", "检测到HybridCLR未安装，是否进行安装？", "安装", "取消"))
                {
                    AutoInstallHybridCLR();
                    return true;
                }
            }
            else
            {
                return true;
            }

            return false;
        }
    }
}