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
            win.position = GUIHelper.GetEditorWindowRect().AlignCenter(440, 750);
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

        string HybridCLRPluginInfo()
        {
            if (IsHybridCLRInstalled())
            {
                return LogColor.Zero2("HybridCLR插件已安装");
            }
            return LogColor.Zero1("HybridCLR插件尚未安装");
        }

        string HybridCLREnvironmentInfo()
        {
            if (IsHybridCLREnvironmentCorrect())
            {
                return LogColor.Zero2("HybridCLR环境已设置");
            }
            return LogColor.Zero1("HybridCLR环境尚未设置");
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _installerController = new InstallerController();                        
        }


        [Title("HybridCLR插件")]
        [InfoBox("$HybridCLRPluginInfo", InfoMessageType = InfoMessageType.None)]
        [PropertyOrder(0)]
        [DisplayAsString(true), HideLabel]
        public string textHybridCLRPluginInfo = "必须先安装插件，才能以HybridCLR执行DLL";

        //[HorizontalGroup("HybridCLR_Plugin")]
        [Button("安装HybridCLR插件", ButtonSizes.Large)]
        [PropertyOrder(1)]
        [DisableIf("IsHybridCLRInstalled")]
        void InstallHybridCLR()
        {
            InstallerWindow.Open();
            //_installerController.InitHybridCLR(_installerController.Il2CppBranch, _installerController.Il2CppInstallDirectory);
        }
        
        //[HorizontalGroup("HybridCLR_Plugin")]
        //[PropertyOrder(1)]
        //[Button("卸载HybridCLR插件", ButtonSizes.Large)]
        //[EnableIf("IsHybridCLRInstalled")]
        //void UninstallHybridCLR()
        //{
        //    HybridCLRUtility.UninstallHybridCLRPlugin();            
        //}

        [Title("HybridCLR环境")]
        [InfoBox("$HybridCLREnvironmentInfo", InfoMessageType = InfoMessageType.None)]
        [PropertyOrder(2)]
        [DisplayAsString(true), HideLabel]
        public string textHybridCLREnvironmentInfo = "参考官方文档<快速开始>";


        [HorizontalGroup("HybridCLR_Environment")]
        [PropertyOrder(3)]
        [Button("设置HybridCLR环境", ButtonSizes.Large)]
        [EnableIf("@false == IsHybridCLREnvironmentCorrect() && IsHybridCLRInstalled()")]
        void InstallEnvironment()
        {
            HybridCLRUtility.SetHybridCLREnvironment();
        }

        [HorizontalGroup("HybridCLR_Environment")]
        [PropertyOrder(3)]
        [Button("清除HybridCLR环境", ButtonSizes.Large)]
        [EnableIf("IsHybridCLREnvironmentCorrect")]
        void UninstallEnvironment()
        {
            HybridCLRUtility.CleanHybridCLREnvironment();
        }

        [Title("AOT-interpreter桥接函数")]
        [PropertyOrder(4)]
        [DisplayAsString(true), HideLabel]
        public string textMethodBridgeInfo = "参考官方文档<AOT-interpreter桥接函数>";

        [PropertyOrder(4)]
        [Button("AOT-interpreter桥接函数生成", ButtonSizes.Large)]
        [EnableIf("IsHybridCLRInstalled")]
        void GenerateMethodBridge()
        {
            //EditorUtility.DisplayProgressBar("", "AOT-interpreter桥接函数生成", 0);

            HybridCLR.Editor.MethodBridgeHelper.GenerateMethodBridgeAll(false);

            //EditorUtility.ClearProgressBar();
            Debug.Log("AOT-interpreter桥接函数生成生成完毕！");
        }

        string CopyAotDllInfo()
        {
            if (!Directory.Exists(HybridCLREditorConst.AOT_DLL_SOURCE_DIR))
            {
                return $"[{HybridCLREditorConst.AOT_DLL_SOURCE_DIR}]中没有DLL文件。需要构建一次主包后才能生成裁剪后的AOT DLL";
            }
            return "Aot Dll将会拷贝到：Assets/Resources/hybrid_clr";
        }

        [TitleGroup("AOT泛型限制(补充元数据)")]
        [PropertyOrder(5)]
        [InfoBox("$CopyAotDllInfo", "@CopyAotDllInfo() != null", InfoMessageType = InfoMessageType.Warning)]
        [DisplayAsString(true), HideLabel]
        public string textCopyAotDllToResourcesInfo = "参考官方文档<AOT泛型限制及原理介绍>";
        //[ListDrawerSettings(Expanded = true, NumberOfItemsPerPage = 5)]
        //[HideLabel]
        //public string[] toCopyAotDllList = new string[]
        //{
        //    "mscorlib.dll",
        //    "System.dll",
        //    "System.Core.dll", // 如果使用了Linq，需要这个
        //};



        [PropertyOrder(5)]        
        [EnableIf("IsHybridCLRInstalled")]
        [Button("拷贝Aot Dll到Resources", ButtonSizes.Large)]
        void CopyAotDll()
        {
            HybridCLRUtility.CopyAotDllToResources();
        }

        [PropertyOrder(6)]
        [Button("打开Aot Dll存放目录", ButtonSizes.Large)]
        void OpenMethodBridgeDir()
        {
            if (false == Directory.Exists(HybridCLREditorConst.AOT_DLL_SOURCE_DIR))
            {
                EditorUtility.DisplayDialog("提示", "目录不存在！", "OK");
                return;
            }
            //打开目录
            ZeroEditorUtility.OpenDirectory(HybridCLREditorConst.AOT_DLL_SOURCE_DIR);
        }

        [TitleGroup("缓存")]
        [PropertyOrder(7)]
        [Button("清理IL2CPP构建缓存目录", ButtonSizes.Large)]
        void CleanIl2CppBuildCache()
        {
            if (!Directory.Exists(HybridCLREditorConst.IL2CPP_BUILD_CACHE_DIR))
            {
                Debug.Log($"清缓存目录不存在");
                return;
            }
            Debug.Log($"清理IL2CPP构建缓存目录:{HybridCLREditorConst.IL2CPP_BUILD_CACHE_DIR}");
            Directory.Delete(HybridCLREditorConst.IL2CPP_BUILD_CACHE_DIR, true);
        }

        [TitleGroup("资料")]
        [PropertyOrder(8)]
        [Button("HybridCLR官方文档", ButtonSizes.Large)]
        void OpenHuaTuoWebSite()
        {
            Application.OpenURL("https://focus-creative-games.github.io/hybridclr/");
        }
    }
}