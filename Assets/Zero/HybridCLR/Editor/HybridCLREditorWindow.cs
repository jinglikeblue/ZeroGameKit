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
        /// ��
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
        ///// �Ƿ�HybridCLR�����װ��
        ///// </summary>
        bool IsHybridCLRInstalled()
        {
            return _installerController.HasInstalledHybridCLR();
        }

        string HybridCLRPluginInfo()
        {
            if (IsHybridCLRInstalled())
            {
                return LogColor.Zero2("HybridCLR����Ѱ�װ");
            }
            return LogColor.Zero1("HybridCLR�����δ��װ");
        }

        string HybridCLREnvironmentInfo()
        {
            if (IsHybridCLREnvironmentCorrect())
            {
                return LogColor.Zero2("HybridCLR����������");
            }
            return LogColor.Zero1("HybridCLR������δ����");
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _installerController = new InstallerController();                        
        }


        [Title("HybridCLR���")]
        [InfoBox("$HybridCLRPluginInfo", InfoMessageType = InfoMessageType.None)]
        [PropertyOrder(0)]
        [DisplayAsString(true), HideLabel]
        public string textHybridCLRPluginInfo = "�����Ȱ�װ�����������HybridCLRִ��DLL";

        //[HorizontalGroup("HybridCLR_Plugin")]
        [Button("��װHybridCLR���", ButtonSizes.Large)]
        [PropertyOrder(1)]
        [DisableIf("IsHybridCLRInstalled")]
        void InstallHybridCLR()
        {
            InstallerWindow.Open();
            //_installerController.InitHybridCLR(_installerController.Il2CppBranch, _installerController.Il2CppInstallDirectory);
        }
        
        //[HorizontalGroup("HybridCLR_Plugin")]
        //[PropertyOrder(1)]
        //[Button("ж��HybridCLR���", ButtonSizes.Large)]
        //[EnableIf("IsHybridCLRInstalled")]
        //void UninstallHybridCLR()
        //{
        //    HybridCLRUtility.UninstallHybridCLRPlugin();            
        //}

        [Title("HybridCLR����")]
        [InfoBox("$HybridCLREnvironmentInfo", InfoMessageType = InfoMessageType.None)]
        [PropertyOrder(2)]
        [DisplayAsString(true), HideLabel]
        public string textHybridCLREnvironmentInfo = "�ο��ٷ��ĵ�<���ٿ�ʼ>";


        [HorizontalGroup("HybridCLR_Environment")]
        [PropertyOrder(3)]
        [Button("����HybridCLR����", ButtonSizes.Large)]
        [EnableIf("@false == IsHybridCLREnvironmentCorrect() && IsHybridCLRInstalled()")]
        void InstallEnvironment()
        {
            HybridCLRUtility.SetHybridCLREnvironment();
        }

        [HorizontalGroup("HybridCLR_Environment")]
        [PropertyOrder(3)]
        [Button("���HybridCLR����", ButtonSizes.Large)]
        [EnableIf("IsHybridCLREnvironmentCorrect")]
        void UninstallEnvironment()
        {
            HybridCLRUtility.CleanHybridCLREnvironment();
        }

        [Title("AOT-interpreter�ŽӺ���")]
        [PropertyOrder(4)]
        [DisplayAsString(true), HideLabel]
        public string textMethodBridgeInfo = "�ο��ٷ��ĵ�<AOT-interpreter�ŽӺ���>";

        [PropertyOrder(4)]
        [Button("AOT-interpreter�ŽӺ�������", ButtonSizes.Large)]
        [EnableIf("IsHybridCLRInstalled")]
        void GenerateMethodBridge()
        {
            //EditorUtility.DisplayProgressBar("", "AOT-interpreter�ŽӺ�������", 0);

            HybridCLR.Editor.MethodBridgeHelper.GenerateMethodBridgeAll(false);

            //EditorUtility.ClearProgressBar();
            Debug.Log("AOT-interpreter�ŽӺ�������������ϣ�");
        }

        string CopyAotDllInfo()
        {
            if (!Directory.Exists(HybridCLREditorConst.AOT_DLL_SOURCE_DIR))
            {
                return $"[{HybridCLREditorConst.AOT_DLL_SOURCE_DIR}]��û��DLL�ļ�����Ҫ����һ��������������ɲü����AOT DLL";
            }
            return "Aot Dll���´������Assets/Resources/hybrid_clr";
        }

        [TitleGroup("AOT��������(����Ԫ����)")]
        [PropertyOrder(5)]
        [InfoBox("$CopyAotDllInfo", "@CopyAotDllInfo() != null", InfoMessageType = InfoMessageType.Warning)]
        [DisplayAsString(true), HideLabel]
        public string textCopyAotDllToResourcesInfo = "�ο��ٷ��ĵ�<AOT�������Ƽ�ԭ�����>";
        //[ListDrawerSettings(Expanded = true, NumberOfItemsPerPage = 5)]
        //[HideLabel]
        //public string[] toCopyAotDllList = new string[]
        //{
        //    "mscorlib.dll",
        //    "System.dll",
        //    "System.Core.dll", // ���ʹ����Linq����Ҫ���
        //};



        [PropertyOrder(5)]        
        [EnableIf("IsHybridCLRInstalled")]
        [Button("����Aot Dll��Resources", ButtonSizes.Large)]
        void CopyAotDll()
        {
            HybridCLRUtility.CopyAotDllToResources();
        }

        [PropertyOrder(6)]
        [Button("��Aot Dll���Ŀ¼", ButtonSizes.Large)]
        void OpenMethodBridgeDir()
        {
            if (false == Directory.Exists(HybridCLREditorConst.AOT_DLL_SOURCE_DIR))
            {
                EditorUtility.DisplayDialog("��ʾ", "Ŀ¼�����ڣ�", "OK");
                return;
            }
            //��Ŀ¼
            ZeroEditorUtility.OpenDirectory(HybridCLREditorConst.AOT_DLL_SOURCE_DIR);
        }

        [TitleGroup("����")]
        [PropertyOrder(7)]
        [Button("����IL2CPP��������Ŀ¼", ButtonSizes.Large)]
        void CleanIl2CppBuildCache()
        {
            if (!Directory.Exists(HybridCLREditorConst.IL2CPP_BUILD_CACHE_DIR))
            {
                Debug.Log($"�建��Ŀ¼������");
                return;
            }
            Debug.Log($"����IL2CPP��������Ŀ¼:{HybridCLREditorConst.IL2CPP_BUILD_CACHE_DIR}");
            Directory.Delete(HybridCLREditorConst.IL2CPP_BUILD_CACHE_DIR, true);
        }

        [TitleGroup("����")]
        [PropertyOrder(8)]
        [Button("HybridCLR�ٷ��ĵ�", ButtonSizes.Large)]
        void OpenHuaTuoWebSite()
        {
            Application.OpenURL("https://focus-creative-games.github.io/hybridclr/");
        }
    }
}