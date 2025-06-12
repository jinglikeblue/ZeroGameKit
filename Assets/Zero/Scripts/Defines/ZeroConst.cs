using Jing;
using System;
using System.IO;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// Zero框架的常量
    /// </summary>
    public class ZeroConst
    {
        /// <summary>
        /// 热更AssetBundle资源的目录名称
        /// </summary>
        public const string AB_DIR_NAME = "ab";

        /// <summary>
        /// 热更AssetBundle资源的目录名称，带分割符
        /// </summary>
        public const string AssetBundleFolderWithSeparator = AB_DIR_NAME + "/";

        /// <summary>
        /// 热更DLL资源的目录名称
        /// </summary>
        public const string DLL_DIR_NAME = "dll";

        /// <summary>
        /// @Scripts中的代码启动类
        /// </summary>
        public const string LOGIC_SCRIPT_STARTUP_CLASS_NAME = "Zero.HotEntrance";

        /// <summary>
        /// @Scripts中的代码启动方法
        /// </summary>
        public const string LOGIC_SCRIPT_STARTUP_METHOD = "Startup";

        /// <summary>
        /// 其它资源文件的目录名称
        /// </summary>
        public const string FILES_DIR_NAME = "files";
        
        /// <summary>
        /// 其它资源文件的目录名称，带分割符
        /// </summary>
        public const string FilesFolderWithSeparator = FILES_DIR_NAME + "/";

        /// <summary>
        /// 热更DLL的文件名称（不含后缀）
        /// </summary>
        public const string DLL_FILE_NAME = "scripts";

        /// <summary>
        /// 资源版本描述文件的名称
        /// </summary>
        public const string RES_JSON_FILE_NAME = "res.json";

        /// <summary>
        /// 启动配置文件名称
        /// </summary>
        public const string SETTING_FILE_NAME = "setting.json";

        /// <summary>
        /// 预置文件压缩包名称
        /// </summary>
        public const string PACKAGE_ZIP_FILE_NAME = "package.zip";

        /// <summary>
        /// 安卓安装包文件名称
        /// </summary>
        public const string ANDROID_APK_NAME = "android_install.apk";

        /// <summary>
        /// AssetBundle文件存储的后缀名
        /// </summary>
        public const string AB_EXTENSION = ".ab";

        /// <summary>
        /// 存储AssetBundle之间依赖关系的manifest文件
        /// </summary>
        public const string MANIFEST_FILE_NAME = "manifest";

        /// <summary>
        /// 直接放在Assets/@ab目录下的资源，会被打包到root_assets.ab文件中
        /// </summary>
        public const string ROOT_AB_FILE_NAME = "root_assets";

        /// <summary>
        /// Resources下的launcher_setting.json资源名
        /// </summary>
        public const string LAUNCHER_SETTING_NAME = "launcher_setting";

        #region 基于项目根目录的路径

        /// <summary>
        /// 项目中的根目录
        /// </summary>
        public const string PROJECT_ASSETS_DIR = "Assets/";

        /// <summary>
        /// 热更资源文件夹名
        /// </summary>
        public const string PROJECT_AB_FOLDER_NAME = "@ab";

        /// <summary>
        /// 热更文件文件夹名
        /// </summary>
        public const string PROJECT_FILES_FOLDER_NAME = "@files";

        /// <summary>
        /// 热更资源在项目中的根目录
        /// </summary>
        public const string PROJECT_AB_DIR = PROJECT_ASSETS_DIR + PROJECT_AB_FOLDER_NAME;

        /// <summary>
        /// 其它热更资源再项目中的根目录
        /// </summary>
        public const string PROJECT_FILES_DIR = PROJECT_ASSETS_DIR + PROJECT_FILES_FOLDER_NAME;

        #endregion

        /// <summary>
        /// Zero框架的Library目录
        /// </summary>
        public const string ZERO_LIBRARY_DIR = "LibraryZero";

        static string _platformDirName = null;

        /// <summary>
        /// 平台目录
        /// </summary>
        public static string PLATFORM_DIR_NAME
        {
            get
            {
                if (null == _platformDirName)
                {
#if UNITY_STANDALONE_WIN
                    _platformDirName = PlatformDirNameConst.PC;
#elif UNITY_STANDALONE_OSX
                    _platformDirName = PlatformDirNameConst.OSX;
#elif UNITY_IPHONE
        _platformDirName = PlatformDirNameConst.IOS;
#elif UNITY_ANDROID
                    _platformDirName = PlatformDirNameConst.ANDROID;
#elif UNITY_WEBGL
                    _platformDirName = PlatformDirNameConst.WebGL;
#elif UNITY_EDITOR //如果上面宏定义都不存在，但是是编辑器，则使用编辑器平台名称
                    _platformDirName = UnityEditor.BuildPipeline.GetBuildTargetName(UnityEditor.EditorUserBuildSettings.activeBuildTarget).ToLower();
#else //如果上面宏定义都不存在，但是是真机环境，则尝试从BuildInfo中获取平台名称
                    _platformDirName = Runtime.BuildInfo.PlatformName;
#endif
                }

                return _platformDirName;
            }
        }

        static string _streamingAssetsPath = null;

        /// <summary>
        /// 可用UnityWebRequest加载资源的streamingAssets目录地址
        /// </summary>
        public static string STREAMING_ASSETS_PATH
        {
            get
            {
                if (null == _streamingAssetsPath)
                {
                    _streamingAssetsPath = Application.streamingAssetsPath;
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_IPHONE
                    //如果在编辑器下，或是PC平台或iOS平台，则要加上file://才能读取资源
                    _streamingAssetsPath = "file://" + _streamingAssetsPath;
#endif
                }

                return _streamingAssetsPath;
            }
        }

        static string _persistentDataPath = null;

        /// <summary>
        /// 可读写目录地址
        /// </summary>
        public static string PERSISTENT_DATA_PATH
        {
            get
            {
                if (null == _persistentDataPath)
                {
                    _persistentDataPath = Application.persistentDataPath;
#if UNITY_EDITOR
                    _persistentDataPath = FileUtility.CombineDirs(false, Directory.GetParent(Application.dataPath).FullName, ZERO_LIBRARY_DIR, "RuntimeCaches");
#elif UNITY_STANDALONE
                _persistentDataPath = FileUtility.CombineDirs(false, Application.dataPath, "Caches");
#endif
                }

                return _persistentDataPath;
            }
        }

        /// <summary>
        /// 网络下载的更新资源存储的目录
        /// </summary>
        public static readonly string WWW_RES_PERSISTENT_DATA_PATH = FileUtility.CombineDirs(false, PERSISTENT_DATA_PATH, "zero", "res");

        /// <summary>
        /// 框架生成文件存放地址
        /// </summary>
        public static readonly string GENERATES_PERSISTENT_DATA_PATH = FileUtility.CombineDirs(false, PERSISTENT_DATA_PATH, "zero", "generated");

        /// <summary>
        /// StreamingAssets下的内嵌资源根目录（不含平台路径)
        /// 内嵌资源根目录
        /// </summary>
        public static readonly string BuiltinResRootFolder = FileUtility.CombinePaths(Application.streamingAssetsPath, "res");

        /// <summary>
        /// StreamingAssets下的内嵌资源根路径，加载AssetBundle时使用；
        /// 使用WWW、UnityWebRequest加载的时候请使用[STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW]；
        /// 举例：[StreamingAssets绝对路径]/res/[平台]。
        /// </summary>
        public static readonly string STREAMING_ASSETS_RES_DATA_PATH = FileUtility.CombinePaths(BuiltinResRootFolder, PLATFORM_DIR_NAME);

        /// <summary>
        /// StreamingAssets下的内嵌资源根路径，使用WWW、UnityWebRequest加载的时候使用
        /// 加载AssetBundle时时候请使用[STREAMING_ASSETS_RES_DATA_PATH]；        
        /// </summary>
        public static readonly string STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW = FileUtility.CombinePaths(STREAMING_ASSETS_PATH, "res", ZeroConst.PLATFORM_DIR_NAME);
    }
}