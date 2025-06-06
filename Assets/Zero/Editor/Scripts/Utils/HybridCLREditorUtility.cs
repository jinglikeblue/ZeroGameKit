using System.IO;
using System.Linq;
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
        [MenuItem("Test/HybridCLR/OneClickForAll")]
        public static async void OneClickForAll()
        {
            if (false == EditorUtility.DisplayDialog("提示", "仅在Build工程之前需要执行！\n耗时较长，是否继续？", "继续", "取消"))
            {
                return;
            }
            
            //检测HybridCLR安装情况
            AutoInstallHybridCLR();
            //生成一次热更DLL代码
            await HotResEditorUtility.GenerateScriptAssembly();
            //生成所有HybridCLR内容
            PrebuildCommand.GenerateAll();
            //拷贝元数据补充需要的程序集
            CopyPatchedAOTAssemblyList();
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