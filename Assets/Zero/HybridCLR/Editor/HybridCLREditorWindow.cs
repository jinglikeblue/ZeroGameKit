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
            return win;
        }

        void SetHybridCLREnvironment()
        {
            HybridCLRUtility.SetHybridCLREnvironment();
        }

        void CleanHybridCLREnvironment()
        {
            HybridCLRUtility.CleanHybridCLREnvironment();
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

        /// <summary>
        /// 是否HybridCLR插件安装了
        /// </summary>
        bool IsHybridCLRInstalled()
        {
            return _installerController.HasInstalledHybridCLR();
        }

        protected override void OnEnable()
        {
            _installerController = new InstallerController();
            isHybridCLRInstalled = IsHybridCLRInstalled();
        }


        [Title("HybridCLR代码插件安装")]
        [LabelText("HybridCLR代码插件是否已安装")]
        [PropertyOrder(0)]
        public bool isHybridCLRInstalled;

        /// <summary>
        /// 安装HybridCLR
        /// </summary>
        [PropertyOrder(1)][DisableIf("IsHybridCLRInstalled")]
        void InstallHybridCLR()
        {
            _installerController.InitHybridCLR(_installerController.Il2CppBranch, _installerController.Il2CppInstallDirectory);
        }

        [PropertyOrder(1)][EnableIf("IsHybridCLRInstalled")]
        void UninstallHybridCLR()
        {
            Directory.Delete(_installerController.Il2CppInstallDirectory, true);            
        }



        [InfoBox("环境变量设置时，打包会走HuaTuo的IL2CPP。否则走的是Unity自己的IL2CPP流程。不使用华佗的情况下，请确保取消该项设置！",InfoMessageType.Warning)]
        [Title("IL2CPP打包环境变量")]
        [PropertyOrder(-3)]
        [ToggleLeft]
        [LabelText("是否HybridCLR已设置并且环境正确")]        
        [ReadOnly]
        [InlineButton("SetHybridCLREnvironment", "设置环境", ShowIf = "IsILTypeIsHybridCLR")]
        [InlineButton("CleanHybridCLREnvironment", "清除环境", ShowIf = "IsILTypeIsHybridCLR")]
        public bool isHybridCLREnvironmentCorrect = HybridCLRUtility.IsHybridCLREnvironmentCorrect; 

        //[PropertySpace(10)]        
        [TitleGroup("元数据补充功能")]
        [PropertyOrder(-2)]
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

        [Button(ButtonSizes.Large), LabelText("AOT-interpreter桥接函数生成")]
        void GenerateMethodBridge()
        {           
            EditorUtility.DisplayProgressBar("", "AOT-interpreter桥接函数生成", 0);

            HybridCLR.Editor.MethodBridgeHelper.GenerateMethodBridgeAll(false);

            EditorUtility.ClearProgressBar();
            Debug.Log("AOT-interpreter桥接函数生成生成完毕！");
        }

        [Button(ButtonSizes.Large), LabelText("打开目录")]
        void OpenMethodBridgeDir()
        {
            //打开目录
            ZeroEditorUtil.OpenDirectory(HybridCLREditorConst.METHOD_BRIDGE_CPP_DIR);
        }

        [TitleGroup("缓存")]        
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
        [Button(ButtonSizes.Large), LabelText("HybridCLR官方文档")]
        void OpenHuaTuoWebSite()
        {
            Application.OpenURL("https://focus-creative-games.github.io/hybridclr/");
        }
    }
}