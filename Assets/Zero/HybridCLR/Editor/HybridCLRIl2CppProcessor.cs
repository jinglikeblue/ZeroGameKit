using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Il2Cpp;
using UnityEngine;
using Zero;

namespace ZeroEditor
{
    public class HybridCLRIl2CppProcessor : IIl2CppProcessor
    {
        public int callbackOrder => 0;

        public void OnBeforeConvertRun(BuildReport report, Il2CppBuildPipelineData data)
        {
#if !UNITY_2021_1_OR_NEWER
            if (null != EnvironmentVariableValue)
            {
                CopyStripDlls(data.target);
            }
#endif
        }

        private void CopyStripDlls(BuildTarget target)
        {
            string stripDllSourceDir = Path.GetDirectoryName(Application.dataPath) + "/" + (target == BuildTarget.Android ? "Temp/StagingArea/assets/bin/Data/Managed" : "Temp/StagingArea/Data/Managed/");            

            if (!Directory.Exists(HybridCLREditorConst.AOT_DLL_SOURCE_DIR))
            {
                Directory.CreateDirectory(HybridCLREditorConst.AOT_DLL_SOURCE_DIR);
            }

            foreach (var dllFile in Directory.GetFiles(stripDllSourceDir, "*.dll"))
            {
                var fileName = Path.GetFileName(dllFile);
                string targetPath = $"{HybridCLREditorConst.AOT_DLL_SOURCE_DIR}/{fileName}";                
                Debug.Log($"拷贝裁剪后的主工程DLL: {dllFile} 到 {targetPath}");
                File.Copy(dllFile, targetPath, true);
            }
        }


        [InitializeOnLoadMethod]
        private static void CheckHybridCLREnvironment()
        {            
            PrefabEditNotice.Ins.onILTypeChanged += (type)=> {
                Debug.Log($"DLL执行方式改变：{type.ToString()}");
                if(type == EILType.HYBRID_CLR)
                {
                    SetEnvironmentVariable();
                }
                else
                {
                    CleanEnvironmentVariable();
                }
            };            
        }

        /// <summary>
        /// 环境变量设置的值，null表示没设置
        /// </summary>
        public static string EnvironmentVariableValue
        {
            get
            {
                return Environment.GetEnvironmentVariable(HybridCLREditorConst.ENVIRONMENT_VARIABLE_KEY);
            }

            private set
            { 
                Environment.SetEnvironmentVariable(HybridCLREditorConst.ENVIRONMENT_VARIABLE_KEY, value);
                if (null == value)
                {
                    Debug.Log(Log.Zero1($"清除了为HybridCLR模式设置的[{HybridCLREditorConst.ENVIRONMENT_VARIABLE_KEY}]环境变量!"));                    
                }
                else
                {
                    Debug.Log(Log.Zero1($"为HybridCLR模式设置了[{HybridCLREditorConst.ENVIRONMENT_VARIABLE_KEY}]环境变量[{HybridCLREditorConst.HYBRID_CLR_IL2CPP_DIR}]"));
                }
            }
        }

        /// <summary>
        /// 设置HybridCLR打包环境
        /// </summary>
        public static void SetEnvironmentVariable()
        {
            PlayerSettingUtility.AddScriptingDefineSymbols(HybridCLREditorConst.SCRIPTING_DEFINE_SYMBOL);
            Debug.Log(Log.Zero1($"HybridCLR模式需要的宏[{HybridCLREditorConst.SCRIPTING_DEFINE_SYMBOL}]已添加!"));

            if (null == EnvironmentVariableValue)
            {
                /// unity允许使用UNITY_IL2CPP_PATH环境变量指定il2cpp的位置，因此我们不再直接修改安装位置的il2cpp，
                /// 而是在本地目录           

                if (!Directory.Exists(HybridCLREditorConst.HYBRID_CLR_IL2CPP_DIR))
                {
                    Debug.LogError(HybridCLREditorConst.HUATUO_INIT_TIP);
                }

                EnvironmentVariableValue = HybridCLREditorConst.HYBRID_CLR_IL2CPP_DIR;
            }          
        }

        /// <summary>
        /// 清除HuaTuo打包环境
        /// </summary>
        public static void CleanEnvironmentVariable() 
        {
            PlayerSettingUtility.RemoveScriptingDefineSymbols(HybridCLREditorConst.SCRIPTING_DEFINE_SYMBOL);
            Debug.Log(Log.Zero1($"HuaTuo模式需要的宏[{HybridCLREditorConst.SCRIPTING_DEFINE_SYMBOL}]已移除!"));

            if (null != EnvironmentVariableValue)
            {
                EnvironmentVariableValue = null;
            }                
        }
    }
}
