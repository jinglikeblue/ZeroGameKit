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
        ///// �Ƿ�HybridCLR�����װ��
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


        [Title("HybridCLR���")]
        [LabelText("HybridCLR����Ƿ��Ѱ�װ")]
        [PropertyOrder(0)]
        public bool isHybridCLRInstalled;

        /// <summary>
        /// ��װHybridCLR
        /// </summary>
        [PropertyOrder(1)]      
        [Button("��װHybridCLR���")]
        //[DisableIf("isHybridCLRInstalled")]
        void InstallHybridCLR()
        {
            _installerController.InitHybridCLR(_installerController.Il2CppBranch, _installerController.Il2CppInstallDirectory);
        }

        /// <summary>
        /// ж��HybridCLR
        /// </summary>
        [PropertyOrder(1)]
        [Button("ж��HybridCLR���")]
        //[EnableIf("isHybridCLRInstalled")]
        void UninstallHybridCLR()
        {
            Directory.Delete(_installerController.Il2CppInstallDirectory, true);
        }

        [Title("HybridCLR����")]
        [PropertyOrder(2)]
        [LabelText("�Ƿ�HybridCLR����������")]
        public bool isHybridCLREnvironmentCorrect;

        [PropertyOrder(3)]
        [Button("����HybridCLR����")]        
        void InstallEnvironment()
        {
            HybridCLRUtility.SetHybridCLREnvironment();
        }

        [PropertyOrder(3)]
        [Button("���HybridCLR����")]        
        void UninstallEnvironment()
        {
            HybridCLRUtility.CleanHybridCLREnvironment();
        }

        [Title("AOT-interpreter�ŽӺ���")]
        [PropertyOrder(4)]
        [Button(ButtonSizes.Large), LabelText("AOT-interpreter�ŽӺ�������")]
        void GenerateMethodBridge()
        {
            EditorUtility.DisplayProgressBar("", "AOT-interpreter�ŽӺ�������", 0);

            HybridCLR.Editor.MethodBridgeHelper.GenerateMethodBridgeAll(false);

            EditorUtility.ClearProgressBar();
            Debug.Log("AOT-interpreter�ŽӺ�������������ϣ�");
        }

        
        [TitleGroup("AOT��������","����Ԫ����")]
        [PropertyOrder(5)]
        [Button(ButtonSizes.Large), LabelText("����Aot Dll��Resources")]
        void CopyAotDll()
        {
            if (!Directory.Exists(HybridCLREditorConst.AOT_DLL_TARGET_DIR))
            {
                Directory.CreateDirectory(HybridCLREditorConst.AOT_DLL_TARGET_DIR);
            }

            var sourceWrongMsg = $"[{HybridCLREditorConst.AOT_DLL_SOURCE_DIR}]��û��DLL�ļ�����Ҫ����һ��������������ɲü����AOT DLL";
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
                Debug.Log($"�ѿ���AOT DLL��{fi.Name}");
            }

            AssetDatabase.Refresh();
        }

        [PropertyOrder(6)]
        [Button(ButtonSizes.Large), LabelText("��Aot Dll���Ŀ¼")]
        void OpenMethodBridgeDir()
        {
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