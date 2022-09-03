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
        static public readonly string AOT_DLL_SOURCE_DIR = $"{Path.GetDirectoryName(Application.dataPath)}/HuatuoData/AssembliesPostIl2CppStrip/{EditorUserBuildSettings.activeBuildTarget}";

        /// <summary>
        /// ������DLL����Ŀ¼
        /// </summary>
        static public readonly string AOT_DLL_TARGET_DIR = $"{Application.dataPath}/Resources/{HuaTuoAotMetadata.HUATUO_RESOURCES_DIR}";

        /// <summary>
        /// AOT-interpreter�ŽӺ����ļ�����Ŀ¼
        /// </summary>
        static public readonly string METHOD_BRIDGE_CPP_DIR = $"{ZeroEditorConst.PROJECT_PATH}/HuatuoData/LocalIl2CppData/il2cpp/libil2cpp/huatuo/interpreter";

        /// <summary>
        /// IL2CPP����Ļ���Ŀ¼
        /// </summary>
        static public readonly string IL2CPP_BUILD_CACHE_DIR = $"{ZeroEditorConst.PROJECT_PATH}/Library/Il2cppBuildCache";

        /// <summary>
        /// HybridCLR����û��������ֶ�
        /// </summary>
        static public readonly string ENVIRONMENT_VARIABLE_KEY = "UNITY_IL2CPP_PATH";

        /// <summary>
        /// HybridCLR�����IL2CPP����ֵ
        /// </summary>
        static public readonly string HYBRID_CLR_IL2CPP_DIR = $"{ZeroEditorConst.PROJECT_PATH}/HuatuoData/LocalIl2CppData/il2cpp";

        /// <summary>
        /// HybridCLR�����IL2CPPĿ¼û�г�ʼ������ʾ
        /// </summary>
        static public readonly string HUATUO_INIT_TIP = $"����il2cppĿ¼:{HYBRID_CLR_IL2CPP_DIR} �����ڣ����ֶ�ִ�� {ZeroEditorConst.PROJECT_PATH}/HuatuoData Ŀ¼�µ� init_local_il2cpp_data.bat ���� init_local_il2cpp_data.sh �ļ�";

        /// <summary>
        /// ʹ��HybridCLR��Ҫ��ӵĺ궨������
        /// </summary>
        public const string SCRIPTING_DEFINE_SYMBOL = "HUATUO_ENABLE";
    }
}