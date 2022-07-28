using Huatuo.Generators;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Zero;
using ZeroEditor;

namespace ZeroEditor
{
    public class HuaTuoEditorConst
    {
        /// <summary>
        /// 主工程DLL来源目录
        /// </summary>
        static public readonly string AOT_DLL_SOURCE_DIR = $"{Path.GetDirectoryName(Application.dataPath)}/HuatuoData/AssembliesPostIl2CppStrip/{EditorUserBuildSettings.activeBuildTarget}";

        /// <summary>
        /// 主工程DLL放置目录
        /// </summary>
        static public readonly string AOT_DLL_TARGET_DIR = $"{Application.dataPath}/Resources/{HuaTuoAotMetadata.HUATUO_RESOURCES_DIR}";

        /// <summary>
        /// AOT-interpreter桥接函数文件生成目录
        /// </summary>
        static public readonly string METHOD_BRIDGE_CPP_DIR = $"{ZeroEditorConst.PROJECT_PATH}/HuatuoData/LocalIl2CppData/il2cpp/libil2cpp/huatuo/interpreter";

        /// <summary>
        /// IL2CPP打包的缓存目录
        /// </summary>
        static public readonly string IL2CPP_BUILD_CACHE_DIR = $"{ZeroEditorConst.PROJECT_PATH}/Library/Il2cppBuildCache";

        /// <summary>
        /// HuaTuo打包用环境参数字段
        /// </summary>
        static public readonly string ENVIRONMENT_VARIABLE_KEY = "UNITY_IL2CPP_PATH";

        /// <summary>
        /// HuaTuo打包用IL2CPP环境值
        /// </summary>
        static public readonly string HUATUO_IL2CPP_DIR = $"{ZeroEditorConst.PROJECT_PATH}/HuatuoData/LocalIl2CppData/il2cpp";

        /// <summary>
        /// HuaTuo打包用IL2CPP目录没有初始化的提示
        /// </summary>
        static public readonly string HUATUO_INIT_TIP = $"本地il2cpp目录:{HUATUO_IL2CPP_DIR} 不存在，请手动执行 {ZeroEditorConst.PROJECT_PATH}/HuatuoData 目录下的 init_local_il2cpp_data.bat 或者 init_local_il2cpp_data.sh 文件";

        /// <summary>
        /// 使用华佗需要添加的宏定义名称
        /// </summary>
        public const string SCRIPTING_DEFINE_SYMBOL = "HUATUO_ENABLE";
    }
}