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
                return Log.Zero2("HybridCLR����Ѱ�װ");
            }
            return Log.Zero1("HybridCLR�����δ��װ");
        }

        string HybridCLREnvironmentInfo()
        {
            if (IsHybridCLREnvironmentCorrect())
            {
                return Log.Zero2("HybridCLR����������");
            }
            return Log.Zero1("HybridCLR������δ����");
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _installerController = new InstallerController();                        
        }


        [Title("HybridCLR���")]
        [InfoBox("$HybridCLRPluginInfo", InfoMessageType = InfoMessageType.None)]                
        [PropertyOrder(1)]      
        [Button("��װHybridCLR���", ButtonSizes.Large)]
        [DisableIf("IsHybridCLRInstalled")]
        void InstallHybridCLR()
        {
            InstallerWindow.Open();
            //_installerController.InitHybridCLR(_installerController.Il2CppBranch, _installerController.Il2CppInstallDirectory);
        }
        
        [HorizontalGroup("HybridCLR_Plugin")]
        [PropertyOrder(1)]
        [Button("ж��HybridCLR���", ButtonSizes.Large)]
        [EnableIf("IsHybridCLRInstalled")]
        void UninstallHybridCLR()
        {            
            Directory.Delete(HybridCLREditorConst.HYBRID_CLR_INSTALL_DIR, true);
        }

        [Title("HybridCLR����")]
        [InfoBox("$HybridCLREnvironmentInfo", InfoMessageType = InfoMessageType.None)]       
        [PropertyOrder(3)]
        [Button("����HybridCLR����", ButtonSizes.Large)]
        [EnableIf("@false == IsHybridCLREnvironmentCorrect() && IsHybridCLRInstalled()")]
        void InstallEnvironment()
        {
            HybridCLRUtility.SetHybridCLREnvironment();
        }

        [PropertyOrder(3)]
        [Button("���HybridCLR����", ButtonSizes.Large)]
        [EnableIf("IsHybridCLREnvironmentCorrect")]
        void UninstallEnvironment()
        {
            HybridCLRUtility.CleanHybridCLREnvironment();
        }

        [Title("AOT-interpreter�ŽӺ���")]
        [PropertyOrder(4)]
        [Button(ButtonSizes.Large), LabelText("AOT-interpreter�ŽӺ�������")]
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
            return "���б�����дҪ������dll";
        }

        [TitleGroup("AOT��������", "����Ԫ����")]
        //[PropertyOrder(5)]
        [InfoBox("$CopyAotDllInfo", "@CopyAotDllInfo() != null", InfoMessageType = InfoMessageType.Warning)]
        //[ListDrawerSettings(Expanded = true, NumberOfItemsPerPage = 5)]
        //[HideLabel]
        //public string[] toCopyAotDllList = new string[]
        //{
        //    "mscorlib.dll",
        //    "System.dll",
        //    "System.Core.dll", // ���ʹ����Linq����Ҫ���
        //};


        //[TitleGroup("AOT��������","����Ԫ����")]
        [PropertyOrder(5)]        
        [EnableIf("IsHybridCLRInstalled")]
        [Button(ButtonSizes.Large), LabelText("����Aot Dll��Resources")]
        void CopyAotDll()
        {
            HybridCLRUtility.CopyAotDllToResources();
        }

        [PropertyOrder(6)]
        [Button(ButtonSizes.Large), LabelText("��Aot Dll���Ŀ¼")]
        void OpenMethodBridgeDir()
        {
            if (false == Directory.Exists(HybridCLREditorConst.AOT_DLL_SOURCE_DIR))
            {
                EditorUtility.DisplayDialog("��ʾ", "Ŀ¼�����ڣ�", "OK");
                return;
            }
            //��Ŀ¼
            ZeroEditorUtil.OpenDirectory(HybridCLREditorConst.AOT_DLL_SOURCE_DIR);
        }

        [TitleGroup("����")]
        [PropertyOrder(7)]
        [Button(ButtonSizes.Large), LabelText("����IL2CPP��������Ŀ¼")]
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
        [Button(ButtonSizes.Large), LabelText("HybridCLR�ٷ��ĵ�")]
        void OpenHuaTuoWebSite()
        {
            Application.OpenURL("https://focus-creative-games.github.io/hybridclr/");
        }
    }
}