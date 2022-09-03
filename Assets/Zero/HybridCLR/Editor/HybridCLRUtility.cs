using HybridCLR.Editor.Installer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Zero;

namespace ZeroEditor
{
    public class HybridCLRUtility
    {
        [InitializeOnLoadMethod]
        public static void InitializeOnLoadMethod()
        {
            Debug.Log(Log.Zero1("HybridCLRUtility:InitializeOnLoadMethod"));
            SyncWithHybridCLRSettings();
            LauncherSetting.onValueChanged += SyncWithHybridCLRSettings;            
        }

        /// <summary>
        /// 根据HybridCLR开关，同步环境
        /// </summary>
        /// <param name="settings"></param>
        static void SyncWithHybridCLRSettings()
        {
            if (IsILTypeIsHybridCLR)
            {
                if (false == IsScriptingDefineSymbolsExist || false == IsEnvironmentVariableExist)
                {
                    SetHybridCLREnvironment();
                }
            }
            else
            {
                if (IsScriptingDefineSymbolsExist || IsEnvironmentVariableExist)
                {
                    CleanHybridCLREnvironment();
                }
            }
        }

        /// <summary>
        /// 是否热更DLL执行模式是HybridCLR
        /// </summary>
        public static bool IsILTypeIsHybridCLR
        {
            get
            {
                var setting = LauncherSetting.Load();
                return setting.ilType == EILType.HYBRID_CLR ? true : false;
            }
        }


        /// <summary>
        /// 是否设置了宏
        /// </summary>
        public static bool IsScriptingDefineSymbolsExist
        {
            get
            {
                return PlayerSettingUtility.CheckScriptingDefineSymbolsExist(HybridCLREditorConst.SCRIPTING_DEFINE_SYMBOL, EditorUserBuildSettings.selectedBuildTargetGroup);
            }
        }

        /// <summary>
        /// 是否设置了环境参数
        /// </summary>
        public static bool IsEnvironmentVariableExist
        {
            get
            {
                return Environment.GetEnvironmentVariable(HybridCLREditorConst.ENVIRONMENT_VARIABLE_KEY) != null ? true : false;
            }
        }

        /// <summary>
        /// 是否HybridCLR已设置并且环境正确
        /// </summary>
        /// <returns></returns>
        public static bool IsHybridCLREnvironmentCorrect
        {
            get
            {
                if (IsILTypeIsHybridCLR && IsScriptingDefineSymbolsExist && IsEnvironmentVariableExist)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 设置HybridCLR模式需要的环境
        /// </summary>
        public static void SetHybridCLREnvironment()
        {
            if (IsILTypeIsHybridCLR)
            {
                if (!Directory.Exists(HybridCLREditorConst.HYBRID_CLR_IL2CPP_DIR))
                {                    
                    Debug.Log(Log.Red($"HybridCLR环境设置失败: HybridCLR插件代码尚未安装[{HybridCLREditorConst.HYBRID_CLR_IL2CPP_DIR}]"));
                    return;
                }

                PlayerSettingUtility.AddScriptingDefineSymbols(HybridCLREditorConst.SCRIPTING_DEFINE_SYMBOL, EditorUserBuildSettings.selectedBuildTargetGroup);

                Debug.Log(Log.Zero1($"HybridCLR环境设置: 添加宏[{HybridCLREditorConst.SCRIPTING_DEFINE_SYMBOL}]"));

                Environment.SetEnvironmentVariable(HybridCLREditorConst.ENVIRONMENT_VARIABLE_KEY, HybridCLREditorConst.HYBRID_CLR_IL2CPP_DIR);

                Debug.Log(Log.Zero1($"HybridCLR环境设置: 设置环境变量[{HybridCLREditorConst.ENVIRONMENT_VARIABLE_KEY} = {HybridCLREditorConst.HYBRID_CLR_IL2CPP_DIR}]"));               

                Debug.Log(Log.Zero1($"HybridCLR环境设置: 拷贝AotDll到[Resources/hybrid_clr]目录，为正式包的补充元数据做准备"));
                CopyAotDllToResources();
            }
            else
            {
                EditorUtility.DisplayDialog("提示", "当前Dll执行方式不是HybridCLR", "OK");
            }
        }

        /// <summary>
        /// 清除HybridCLR模式需要的环境
        /// </summary>
        public static void CleanHybridCLREnvironment()
        {
            if (false == IsILTypeIsHybridCLR)
            {
                PlayerSettingUtility.RemoveScriptingDefineSymbols(HybridCLREditorConst.SCRIPTING_DEFINE_SYMBOL, EditorUserBuildSettings.selectedBuildTargetGroup);

                Debug.Log(Log.Zero1($"HybridCLR环境清除: 删除宏[{HybridCLREditorConst.SCRIPTING_DEFINE_SYMBOL}]"));

                Environment.SetEnvironmentVariable(HybridCLREditorConst.ENVIRONMENT_VARIABLE_KEY, null);

                Debug.Log(Log.Zero1($"HybridCLR环境清除: 删除环境变量[{HybridCLREditorConst.ENVIRONMENT_VARIABLE_KEY}]"));

                Debug.Log(Log.Zero1($"HybridCLR环境清除:  删除为正式包的补充元数据做准备的[Resources/hybrid_clr]目录"));
                DeleteAotDllResourcesDir();
            }
            else
            {
                EditorUtility.DisplayDialog("提示", "当前Dll执行方式是HybridCLR", "OK");
            }
        }

        /// <summary>
        /// 拷贝AotDll到Resources/hybrid_clr目录，为正式包的补充元数据做准备
        /// </summary>
        public static void CopyAotDllToResources()
        {            
            var sourceWrongMsg = $"[{HybridCLREditorConst.AOT_DLL_SOURCE_DIR}]中没有DLL文件。需要构建一次主包后才能生成裁剪后的AOT DLL";
            if (!Directory.Exists(HybridCLREditorConst.AOT_DLL_SOURCE_DIR))
            {
                Debug.LogError(sourceWrongMsg);
                return;
            }

            //删除老文件夹中的所有内容
            DeleteAotDllResourcesDir();

            Directory.CreateDirectory(HybridCLREditorConst.AOT_DLL_TARGET_DIR);

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

        /// <summary>
        /// 删除为正式包的补充元数据做准备的"Resources/hybrid_clr"目录
        /// </summary>
        public static void DeleteAotDllResourcesDir()
        {
            Directory.Delete(HybridCLREditorConst.AOT_DLL_TARGET_DIR, true);
        }

        /// <summary>
        /// 卸载HybridCLR插件
        /// </summary>
        public static void UninstallHybridCLRPlugin()
        {
            Directory.Delete(HybridCLREditorConst.HYBRID_CLR_INSTALL_DIR, true);
            Debug.Log(Log.Zero1($"HybridCLR插件卸载: 删除插件目录[{HybridCLREditorConst.HYBRID_CLR_INSTALL_DIR}]"));

            CleanHybridCLREnvironment();
        }
    }
}
