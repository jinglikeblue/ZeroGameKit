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

        [InfoBox("环境变量设置时，打包会走HuaTuo的IL2CPP。否则走的是Unity自己的IL2CPP流程。不使用华佗的情况下，请确保取消该项设置！",InfoMessageType.Warning)]
        [Title("IL2CPP打包环境变量")]
        [PropertyOrder(-3)]
        [ToggleLeft]
        [LabelText("[UNITY_IL2CPP_PATH]是否设置")]
        [OnValueChanged("OnSetEnvironmentVariable")]
        public bool isSetEnvironmentVariable; 

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

        [TitleGroup("AOT-interpreter桥接函数生成")]
        [LabelText("x64"), ToggleLeft]
        public bool isGenerateMethodBridge_X64 = true;
        [TitleGroup("AOT-interpreter桥接函数生成")]
        [LabelText("arm64"), ToggleLeft]
        public bool isGenerateMethodBridge_Arm64 = true;

        [Button(ButtonSizes.Large), LabelText("生成")]
        void GenerateMethodBridge()
        {
            EditorUtility.DisplayProgressBar("AOT-interpreter桥接函数生成", "x64", 0);

            if (isGenerateMethodBridge_X64)
            {
                string outputFile = $"{HybridCLREditorConst.METHOD_BRIDGE_CPP_DIR}/MethodBridge_x64.cpp";
                //GenerateMethodBridge(outputFile, CallConventionType.X64);
            }

            EditorUtility.DisplayProgressBar("AOT-interpreter桥接函数生成", "arm64", 0);

            if (isGenerateMethodBridge_Arm64)
            {
                string outputFile = $"{HybridCLREditorConst.METHOD_BRIDGE_CPP_DIR}/MethodBridge_arm64.cpp";
                //GenerateMethodBridge(outputFile, CallConventionType.Arm64);
            }

            EditorUtility.DisplayProgressBar("AOT-interpreter桥接函数生成", "清理IL2CPP构建缓存目录", 1);
            CleanIl2CppBuildCache();

            EditorUtility.ClearProgressBar();
            Debug.Log("生成完毕！");
        }

        [Button(ButtonSizes.Large), LabelText("打开目录")]
        void OpenMethodBridgeDir()
        {
            //打开目录
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
        //    Debug.Log($"AOT-interpreter桥接函数生成: {outputFile}");
        //}


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
        [Button(ButtonSizes.Large), LabelText("访问HuaTuo官网")]
        void OpenHuaTuoWebSite()
        {
            Application.OpenURL("https://focus-creative-games.github.io/hybridclr/");
        }
    }
}