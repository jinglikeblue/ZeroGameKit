using HybridCLR.Editor.Installer;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Zero;

namespace ZeroEditor
{
    public class HybridCLREditorWindow : OdinEditorWindow
    {
        /// <summary>
        /// 打开
        /// </summary>
        public static HybridCLREditorWindow Open()
        {            
            var win = GetWindow<HybridCLREditorWindow>("HybridCLR");            
            win.position = GUIHelper.GetEditorWindowRect().AlignCenter(440, 470);
            win.Show();
            return win;
        }

        bool IsILTypeIsHybridCLR()
        {
            return HybridCLRUtility.IsILTypeIsHybridCLR;
        }

        bool IsHybridCLREnvironmentCorrect()
        {
            return HybridCLRUtility.IsHybridCLREnvironmentCorrect;
        }

        InstallerController _installerController;

        ///// <summary>
        ///// 是否HybridCLR插件安装了
        ///// </summary>
        bool IsHybridCLRInstalled()
        {
            return _installerController.HasInstalledHybridCLR();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _installerController = new InstallerController();
            isHybridCLRInstalled = IsHybridCLRInstalled();
            isHybridCLREnvironmentCorrect = IsHybridCLREnvironmentCorrect();
        }


        [Title("HybridCLR插件")]
        [LabelText("HybridCLR插件是否已安装")]
        [PropertyOrder(0)]
        public bool isHybridCLRInstalled;

        /// <summary>
        /// 安装HybridCLR
        /// </summary>
        [PropertyOrder(1)]      
        [Button("安装HybridCLR插件")]
        //[DisableIf("isHybridCLRInstalled")]
        void InstallHybridCLR()
        {
            _installerController.InitHybridCLR(_installerController.Il2CppBranch, _installerController.Il2CppInstallDirectory);
        }

        /// <summary>
        /// 卸载HybridCLR
        /// </summary>
        [PropertyOrder(1)]
        [Button("卸载HybridCLR插件")]
        //[EnableIf("isHybridCLRInstalled")]
        void UninstallHybridCLR()
        {
            Directory.Delete(_installerController.Il2CppInstallDirectory, true);
        }

        [Title("HybridCLR环境")]
        [PropertyOrder(2)]
        [LabelText("是否HybridCLR环境已设置")]
        public bool isHybridCLREnvironmentCorrect;

        [PropertyOrder(3)]
        [Button("设置HybridCLR环境")]        
        void InstallEnvironment()
        {
            HybridCLRUtility.SetHybridCLREnvironment();
        }

        [PropertyOrder(3)]
        [Button("清除HybridCLR环境")]        
        void UninstallEnvironment()
        {
            HybridCLRUtility.CleanHybridCLREnvironment();
        }

        [Title("AOT-interpreter桥接函数")]
        [PropertyOrder(4)]
        [Button(ButtonSizes.Large), LabelText("AOT-interpreter桥接函数生成")]
        void GenerateMethodBridge()
        {
            EditorUtility.DisplayProgressBar("", "AOT-interpreter桥接函数生成", 0);

            HybridCLR.Editor.MethodBridgeHelper.GenerateMethodBridgeAll(false);

            EditorUtility.ClearProgressBar();
            Debug.Log("AOT-interpreter桥接函数生成生成完毕！");
        }

        
        [TitleGroup("AOT泛型限制","补充元数据")]
        [PropertyOrder(5)]
        [Button(ButtonSizes.Large), LabelText("拷贝Aot Dll到Resources")]
        void CopyAotDll()
        {
            if (!Directory.Exists(HybridCLREditorConst.AOT_DLL_TARGET_DIR))
            {
                Directory.CreateDirectory(HybridCLREditorConst.AOT_DLL_TARGET_DIR);
            }

            var sourceWrongMsg = $"[{HybridCLREditorConst.AOT_DLL_SOURCE_DIR}]中没有DLL文件。需要构建一次主包后才能生成裁剪后的AOT DLL";
            if (!Directory.Exists(HybridCLREditorConst.AOT_DLL_SOURCE_DIR))
            {
                Debug.LogError(sourceWrongMsg);
                return;
            }

            var dllFileList = Directory.GetFiles(HybridCLREditorConst.AOT_DLL_SOURCE_DIR, "*.dll");

            if (0 == dllFileList.Length)
            {
                Debug.LogError(sourceWrongMsg);
                return;
            }

            foreach (var dllFile in dllFileList)
            {
                var fi = new FileInfo(dllFile);
                string dllBytesFile = $"{HybridCLREditorConst.AOT_DLL_TARGET_DIR}/{fi.Name}.bytes";
                File.Copy(dllFile, dllBytesFile, true);
                Debug.Log($"已拷贝AOT DLL：{fi.Name}");
            }

            AssetDatabase.Refresh();
        }

        [PropertyOrder(6)]
        [Button(ButtonSizes.Large), LabelText("打开Aot Dll存放目录")]
        void OpenMethodBridgeDir()
        {
            //打开目录
            ZeroEditorUtil.OpenDirectory(HybridCLREditorConst.AOT_DLL_SOURCE_DIR);
        }

        [TitleGroup("缓存")]
        [PropertyOrder(7)]
        [Button(ButtonSizes.Large), LabelText("清理IL2CPP构建缓存目录")]
        void CleanIl2CppBuildCache()
        {
            if (!Directory.Exists(HybridCLREditorConst.IL2CPP_BUILD_CACHE_DIR))
            {
                return;
            }
            Debug.Log($"清理IL2CPP构建缓存目录:{HybridCLREditorConst.IL2CPP_BUILD_CACHE_DIR}");
            Directory.Delete(HybridCLREditorConst.IL2CPP_BUILD_CACHE_DIR, true);
        }

        [TitleGroup("资料")]
        [PropertyOrder(8)]
        [Button(ButtonSizes.Large), LabelText("HybridCLR官方文档")]
        void OpenHuaTuoWebSite()
        {
            Application.OpenURL("https://focus-creative-games.github.io/hybridclr/");
        }
    }
}