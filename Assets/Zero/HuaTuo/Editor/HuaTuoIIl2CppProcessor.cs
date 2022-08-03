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
    public class HuaTuoIIl2CppProcessor : IIl2CppProcessor
    {
        public int callbackOrder => 0;

        public void OnBeforeConvertRun(BuildReport report, Il2CppBuildPipelineData data)
        {
#if !UNITY_2021_1_OR_NEWER
            CopyStripDlls(data.target);
#endif
        }

        private void CopyStripDlls(BuildTarget target)
        {
            string stripDllSourceDir = Path.GetDirectoryName(Application.dataPath) + "/" + (target == BuildTarget.Android ? "Temp/StagingArea/assets/bin/Data/Managed" : "Temp/StagingArea/Data/Managed/");            

            if (!Directory.Exists(HuaTuoEditorConst.AOT_DLL_SOURCE_DIR))
            {
                Directory.CreateDirectory(HuaTuoEditorConst.AOT_DLL_SOURCE_DIR);
            }

            foreach (var dllFile in Directory.GetFiles(stripDllSourceDir, "*.dll"))
            {
                var fileName = Path.GetFileName(dllFile);
                string targetPath = $"{HuaTuoEditorConst.AOT_DLL_SOURCE_DIR}/{fileName}";                
                Debug.Log($"�����ü����������DLL: {dllFile} �� {targetPath}");
                File.Copy(dllFile, targetPath, true);
            }
        }


        [InitializeOnLoadMethod]
        private static void CheckHuaTuoEnvironment()
        {            
            PrefabEditNotice.Ins.onILTypeChanged += (type)=> {
                Debug.Log($"DLLִ�з�ʽ�ı䣺{type.ToString()}");
                if(type == EILType.HUA_TUO)
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
        /// �����������õ�ֵ��null��ʾû����
        /// </summary>
        public static string EnvironmentVariableValue
        {
            get
            {
                return Environment.GetEnvironmentVariable(HuaTuoEditorConst.ENVIRONMENT_VARIABLE_KEY);
            }

            private set
            { 
                Environment.SetEnvironmentVariable(HuaTuoEditorConst.ENVIRONMENT_VARIABLE_KEY, value);
                if (null == value)
                {
                    Debug.Log(Log.Zero1($"�����ΪHuaTuoģʽ���õ�[{HuaTuoEditorConst.ENVIRONMENT_VARIABLE_KEY}]��������!"));                    
                }
                else
                {
                    Debug.Log(Log.Zero1($"ΪHuaTuoģʽ������[{HuaTuoEditorConst.ENVIRONMENT_VARIABLE_KEY}]��������[{HuaTuoEditorConst.HUATUO_IL2CPP_DIR}]"));
                }
            }
        }

        /// <summary>
        /// ����HuaTuo�������
        /// </summary>
        public static void SetEnvironmentVariable()
        {
            PlayerSettingUtility.AddScriptingDefineSymbols(HuaTuoEditorConst.SCRIPTING_DEFINE_SYMBOL);
            Debug.Log(Log.Zero1($"HuaTuoģʽ��Ҫ�ĺ�[{HuaTuoEditorConst.SCRIPTING_DEFINE_SYMBOL}]�����!"));

            if (null == EnvironmentVariableValue)
            {
                /// unity����ʹ��UNITY_IL2CPP_PATH��������ָ��il2cpp��λ�ã�������ǲ���ֱ���޸İ�װλ�õ�il2cpp��
                /// �����ڱ���Ŀ¼           

                if (!Directory.Exists(HuaTuoEditorConst.HUATUO_IL2CPP_DIR))
                {
                    Debug.LogError(HuaTuoEditorConst.HUATUO_INIT_TIP);
                }

                EnvironmentVariableValue = HuaTuoEditorConst.HUATUO_IL2CPP_DIR;
            }          
        }

        /// <summary>
        /// ���HuaTuo�������
        /// </summary>
        public static void CleanEnvironmentVariable() 
        {
            PlayerSettingUtility.RemoveScriptingDefineSymbols(HuaTuoEditorConst.SCRIPTING_DEFINE_SYMBOL);
            Debug.Log(Log.Zero1($"HuaTuoģʽ��Ҫ�ĺ�[{HuaTuoEditorConst.SCRIPTING_DEFINE_SYMBOL}]���Ƴ�!"));

            if (null != EnvironmentVariableValue)
            {
                EnvironmentVariableValue = null;
            }                
        }
    }
}
