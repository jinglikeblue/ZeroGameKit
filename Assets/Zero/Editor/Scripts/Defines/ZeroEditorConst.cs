using Jing;
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
    /// <summary>
    /// Zero编辑器的常量
    /// </summary>
    public class ZeroEditorConst
    {
        /// <summary>
        /// 项目目录
        /// </summary>
        public static readonly string PROJECT_PATH = Directory.GetParent(Application.dataPath)?.FullName;

        /// <summary>
        /// 编辑器配置文件目录
        /// </summary>
        public static readonly string EDITOR_CONFIG_DIR = FileUtility.CombineDirs(false, ZeroConst.ZERO_LIBRARY_DIR, "EditorConfigs");
        
        /// <summary>
        /// 热更资源发布目录
        /// </summary>
        public static readonly string PUBLISH_RES_ROOT_DIR = FileUtility.CombineDirs(false, ZeroConst.ZERO_LIBRARY_DIR, "Release", "res", ZeroConst.PLATFORM_DIR_NAME);
        
        /// <summary>
        /// DLL打包的发布目录
        /// </summary>
        public static readonly string DLL_PUBLISH_DIR = FileUtility.CombineDirs(false, PUBLISH_RES_ROOT_DIR, ZeroConst.DLL_DIR_NAME);

        /// <summary>
        /// AssetBundle打包的发布目录
        /// </summary>
        public static readonly string ASSET_BUNDLE_PUBLISH_DIR = FileUtility.CombineDirs(false, PUBLISH_RES_ROOT_DIR, ZeroConst.AB_DIR_NAME);

        /// <summary>
        /// Files资源的发布目录
        /// </summary>
        public static readonly string FILES_PUBLISH_DIR = FileUtility.CombineDirs(false, PUBLISH_RES_ROOT_DIR, ZeroConst.FILES_DIR_NAME);

        /// <summary>
        /// DD打包缓存目录
        /// </summary>
        public static readonly string DLL_CACHE_DIR = FileUtility.CombineDirs(false, ZeroConst.ZERO_LIBRARY_DIR, "ReleaseCache", ZeroConst.DLL_DIR_NAME);

        /// <summary>
        /// AssetBundle打包缓存目录
        /// </summary>
        public static readonly string ASSET_BUNDLE_CACHE_DIR = FileUtility.CombineDirs(false, ZeroConst.ZERO_LIBRARY_DIR, "ReleaseCache", ZeroConst.AB_DIR_NAME);

        /// <summary>
        /// 热更代码的根目录
        /// </summary>
        public const string HOT_SCRIPT_ROOT_DIR = "Assets/@Scripts";

        /// <summary>
        /// 当前发布平台
        /// </summary>
        public static BuildTarget BUILD_PLATFORM => EditorUserBuildSettings.activeBuildTarget;
    }
}