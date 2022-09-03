using HybridCLR.Editor;
using System.IO;
using UnityEditor;
using UnityEngine;
using Zero;

namespace ZeroEditor
{
    public class HybridCLREditorConst
    {
        /// <summary>
        /// 主工程DLL来源目录
        /// </summary>
        static public readonly string AOT_DLL_SOURCE_DIR = BuildConfig.GetAssembliesPostIl2CppStripDir(EditorUserBuildSettings.activeBuildTarget);

        /// <summary>
        /// 主工程DLL放置目录
        /// </summary>
        static public readonly string AOT_DLL_TARGET_DIR = $"{Application.dataPath}/Resources/{HybridCLRAotMetadata.AOT_DLL_RESOURCES_DIR}";

        /// <summary>
        /// AOT-interpreter桥接函数文件生成目录
        /// </summary>
        static public readonly string METHOD_BRIDGE_CPP_DIR = BuildConfig.MethodBridgeCppDir;

        /// <summary>
        /// IL2CPP打包的缓存目录
        /// </summary>
        static public readonly string IL2CPP_BUILD_CACHE_DIR = BuildConfig.Il2CppBuildCacheDir;

        /// <summary>
        /// HybridCLR打包用环境参数字段
        /// </summary>
        static public readonly string ENVIRONMENT_VARIABLE_KEY = "UNITY_IL2CPP_PATH";

        /// <summary>
        /// HybridCLR打包用IL2CPP环境值
        /// </summary>
        static public readonly string HYBRID_CLR_IL2CPP_DIR = BuildConfig.LocalIl2CppDir;        

        /// <summary>
        /// 使用HybridCLR需要添加的宏定义名称
        /// </summary>
        public const string SCRIPTING_DEFINE_SYMBOL = "HYBRID_CLR_ENABLE";
    }
}