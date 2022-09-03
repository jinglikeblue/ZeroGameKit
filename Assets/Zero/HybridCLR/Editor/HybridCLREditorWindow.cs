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
            var win = GetWindow<HybridCLREditorWindow>("HuaTuo");
            win.position = GUIHelper.GetEditorWindowRect().AlignCenter(440, 470);
            return win;
        }

        protected override void OnEnable()
        {
            isSetEnvironmentVariable = HybridCLRUtility.EnvironmentVariableValue == null ? false : true;
            PrefabEditNotice.Ins.onILTypeChanged += OnILTypeChanged;
        }

        private void OnDisable()
        {
            PrefabEditNotice.Ins.onILTypeChanged -= OnILTypeChanged;
        }

        private void OnILTypeChanged(EILType type)
        {
            isSetEnvironmentVariable = type == EILType.HYBRID_CLR ? true : false;
        }

        void OnSetEnvironmentVariable()
        {
            if(isSetEnvironmentVariable)
            {
                HybridCLRUtility.SetEnvironmentVariable();
            }
            else
            {
                HybridCLRUtility.CleanEnvironmentVariable();
            }            
        }

        [InfoBox("������������ʱ���������HuaTuo��IL2CPP�������ߵ���Unity�Լ���IL2CPP���̡���ʹ�û�٢������£���ȷ��ȡ���������ã�",InfoMessageType.Warning)]
        [Title("IL2CPP�����������")]
        [PropertyOrder(-3)]
        [ToggleLeft]
        [LabelText("[UNITY_IL2CPP_PATH]�Ƿ�����")]
        [OnValueChanged("OnSetEnvironmentVariable")]
        public bool isSetEnvironmentVariable; 

        //[PropertySpace(10)]        
        [TitleGroup("Ԫ���ݲ��书��")]
        [PropertyOrder(-2)]
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

        [TitleGroup("AOT-interpreter�ŽӺ�������")]
        [LabelText("x64"), ToggleLeft]
        public bool isGenerateMethodBridge_X64 = true;
        [TitleGroup("AOT-interpreter�ŽӺ�������")]
        [LabelText("arm64"), ToggleLeft]
        public bool isGenerateMethodBridge_Arm64 = true;

        [Button(ButtonSizes.Large), LabelText("����")]
        void GenerateMethodBridge()
        {
            EditorUtility.DisplayProgressBar("AOT-interpreter�ŽӺ�������", "x64", 0);

            if (isGenerateMethodBridge_X64)
            {
                string outputFile = $"{HybridCLREditorConst.METHOD_BRIDGE_CPP_DIR}/MethodBridge_x64.cpp";
                //GenerateMethodBridge(outputFile, CallConventionType.X64);
            }

            EditorUtility.DisplayProgressBar("AOT-interpreter�ŽӺ�������", "arm64", 0);

            if (isGenerateMethodBridge_Arm64)
            {
                string outputFile = $"{HybridCLREditorConst.METHOD_BRIDGE_CPP_DIR}/MethodBridge_arm64.cpp";
                //GenerateMethodBridge(outputFile, CallConventionType.Arm64);
            }

            EditorUtility.DisplayProgressBar("AOT-interpreter�ŽӺ�������", "����IL2CPP��������Ŀ¼", 1);
            CleanIl2CppBuildCache();

            EditorUtility.ClearProgressBar();
            Debug.Log("������ϣ�");
        }

        [Button(ButtonSizes.Large), LabelText("��Ŀ¼")]
        void OpenMethodBridgeDir()
        {
            //��Ŀ¼
            ZeroEditorUtil.OpenDirectory(HybridCLREditorConst.METHOD_BRIDGE_CPP_DIR);
        }

        //void GenerateMethodBridge(string outputFile, CallConventionType cct)
        //{            
        //    var fi = new FileInfo(outputFile);
        //    if (false == fi.Directory.Exists)
        //    {
        //        fi.Directory.Create();
        //    }

        //    var g = new MethodBridgeGenerator(new MethodBridgeGeneratorOptions()
        //    {
        //        CallConvention = cct,
        //        Assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList(),
        //        OutputFile = outputFile,
        //    });

        //    g.PrepareMethods();
        //    g.Generate();            
        //    Debug.Log($"AOT-interpreter�ŽӺ�������: {outputFile}");
        //}


        [TitleGroup("����")]        
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
        [Button(ButtonSizes.Large), LabelText("����HuaTuo����")]
        void OpenHuaTuoWebSite()
        {
            Application.OpenURL("https://focus-creative-games.github.io/hybridclr/");
        }
    }
}