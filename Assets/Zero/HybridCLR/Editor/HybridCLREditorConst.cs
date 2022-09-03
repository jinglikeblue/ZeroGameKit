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
        /// ������DLL��ԴĿ¼
        /// </summary>
        static public readonly string AOT_DLL_SOURCE_DIR = BuildConfig.GetAssembliesPostIl2CppStripDir(EditorUserBuildSettings.activeBuildTarget);

        /// <summary>
        /// ������DLL����Ŀ¼
        /// </summary>
        static public readonly string AOT_DLL_TARGET_DIR = $"{Application.dataPath}/Resources/{HybridCLRAotMetadata.AOT_DLL_RESOURCES_DIR}";

        /// <summary>
        /// AOT-interpreter�ŽӺ����ļ�����Ŀ¼
        /// </summary>
        static public readonly string METHOD_BRIDGE_CPP_DIR = BuildConfig.MethodBridgeCppDir;

        /// <summary>
        /// IL2CPP����Ļ���Ŀ¼
        /// </summary>
        static public readonly string IL2CPP_BUILD_CACHE_DIR = BuildConfig.Il2CppBuildCacheDir;

        /// <summary>
        /// HybridCLR����û��������ֶ�
        /// </summary>
        static public readonly string ENVIRONMENT_VARIABLE_KEY = "UNITY_IL2CPP_PATH";

        /// <summary>
        /// HybridCLR�����IL2CPP����ֵ
        /// </summary>
        static public readonly string HYBRID_CLR_IL2CPP_DIR = BuildConfig.LocalIl2CppDir;        

        /// <summary>
        /// ʹ��HybridCLR��Ҫ��ӵĺ궨������
        /// </summary>
        public const string SCRIPTING_DEFINE_SYMBOL = "HYBRID_CLR_ENABLE";
    }
}