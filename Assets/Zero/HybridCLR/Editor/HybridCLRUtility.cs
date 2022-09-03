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
            SyncWithHybridCLRSettings(null);
            HybridCLRSettings.Ins.onEnableChanged += SyncWithHybridCLRSettings;
        }

        /// <summary>
        /// 根据HybridCLR开关，同步环境
        /// </summary>
        /// <param name="settings"></param>
        static void SyncWithHybridCLRSettings(HybridCLRSettings settings)
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
                return HybridCLRSettings.Ins.IsHybridCLREnable;
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
                if (HybridCLRSettings.Ins.IsHybridCLREnable && IsScriptingDefineSymbolsExist && IsEnvironmentVariableExist)
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
            }
        }
    }
}
