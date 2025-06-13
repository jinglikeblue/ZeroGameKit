using System;
using Jing;
using System.IO;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 「Zero」框架运行时
    /// </summary>
    public static class Runtime
    {
        /// <summary>
        /// 构建单例
        /// </summary>
        // public static readonly Runtime Ins = new Runtime();

        internal static BuildInfo BuildInfo { get; private set; }

        /// <summary>
        /// 是否启用了IL2Cpp
        /// </summary>
        public static bool IsIL2CPP => BuildInfo.IsIL2CPP;

        /// <summary>
        /// 是否启用了HybridCLR
        /// </summary>
        public static bool IsHybridClrEnable => BuildInfo.IsHybridClrEnable;

        /// <summary>
        /// 平台名称
        /// </summary>
        public static string PlatformName => BuildInfo.PlatformName;

        /// <summary>
        /// 启动器设置数据
        /// </summary>
        internal static LauncherSettingData LauncherData;

        /// <summary>
        /// 内嵌资源初始化器
        /// </summary>
        internal static readonly BuiltinInitiator BuiltinInitiator = new BuiltinInitiator();

        /// <summary>
        /// 是否存在内嵌资源
        /// </summary>
        public static bool IsBuiltinResExist => BuiltinInitiator.IsBuiltinResVerExist;

        /// <summary>
        /// 资源模式是否仅使用包内资源（离线资源模式）
        /// </summary>
        public static bool IsOnlyUseBuiltinRes => false == LauncherData.isHotPatchEnable;

        /// <summary>
        /// 是否允许离线运行
        /// </summary>
        public static bool IsOfflineEnable => LauncherData.isOfflineEnable;

        /// <summary>
        /// 是否用DLL方式启动程序
        /// </summary>
        public static bool IsUseDll => LauncherData.isUseDll;

        /// <summary>
        /// 本地数据
        /// </summary>
        public static LocalDataModel localData { get; private set; }

        /// <summary>
        /// 配置
        /// </summary>
        public static SettingVO setting;

        /// <summary>
        /// 网络资源目录列表
        /// </summary>
        public static string[] SettingFileNetDirList { get; private set; }

        /// <summary>
        /// 基于运行平台的网络资源目录(使用的网络资源的地址)
        /// 举例: [资源所在URL地址]/res/[平台]
        /// </summary>
        public static string netResDir;

        /// <summary>
        /// 资源对象版本数据
        /// </summary>
        public static NetResVerModel netResVer;

        /// <summary>
        /// 存放下载文件的目录
        /// 举例: [资源所在目录绝对路径]/res
        /// </summary>
        public static string localResDir { get; private set; }

        /// <summary>
        /// 本地的资源版本
        /// </summary>
        public static BaseWriteableResVerModel localResVer { get; private set; }

        /// <summary>
        /// 获取运行时所有的资源版本数据
        /// </summary>
        public static ResVerModel resVer => IsHotResEnable ? netResVer : localResVer;

        /// <summary>
        /// Zero框架生成的文件的目录
        /// </summary>
        public static string generateFilesDir { get; private set; }

        /// <summary>
        /// 是否使用AssetBundle加载资源
        /// </summary>
        public static bool IsUseAssetBundle => LauncherData.isUseAssetBundle;

        /// <summary>
        /// 是否使用AssetDataBase加载资源
        /// </summary>
        public static bool IsUseAssetDataBase => !LauncherData.isUseAssetBundle;

        /// <summary>
        /// 运行时是否依赖网络（需要更新资源）
        /// </summary>
        public static bool IsNeedNetwork => LauncherData.isUseAssetBundle && LauncherData.isHotPatchEnable;

        /// <summary>
        /// 运行时是否允许加载热更目录中的资源 
        /// </summary>
        public static bool IsHotResEnable => LauncherData.isUseAssetBundle && LauncherData.isHotPatchEnable;

        static Runtime()
        {
            BuildInfo = BuildInfo.TryLoadBuildInfo();
        }

        /// <summary>
        /// 设置是否打印日志
        /// </summary>
        /// <param name="enable"></param>
        internal static void SetLogEnable(bool enable)
        {
            LauncherData.isLogEnable = enable;
            //日志控制
            Debug.unityLogger.logEnabled = enable;
        }

        internal static void Init(LauncherSettingData data)
        {
            if (null != LauncherData)
            {
                Debug.LogError("Runtime无法重复初始化!");
                return;
            }

            LauncherData = data;

            SetLogEnable(data.isLogEnable);

            InitHotResRuntime();

            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.LinuxEditor:
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.WebGLPlayer:
                    break;
                default:
                    Debug.LogWarning($"[Zero] Zero未对当前平台({PlatformName})进行优化，请自行测试。");
                    break;
            }

            generateFilesDir = ZeroConst.GENERATES_PERSISTENT_DATA_PATH;
            if (false == Directory.Exists(generateFilesDir))
            {
                Directory.CreateDirectory(generateFilesDir);
            }

            localData = new LocalDataModel();
            if (BuiltinInitiator.IsBuiltinResVerExist)
            {
                localResVer = new LocalMixResVerModel(BuiltinInitiator.ResVer);
            }
            else
            {
                localResVer = new LocalResVerModel();
            }
        }

        /// <summary>
        /// 初始化热更资源相关的运行参数
        /// </summary>
        private static void InitHotResRuntime()
        {
            SettingFileNetDirList = new string[LauncherData.urlRoots.Length];
            for (var i = 0; i < SettingFileNetDirList.Length; i++)
            {
                SettingFileNetDirList[i] = FileUtility.CombineDirs(false, LauncherData.urlRoots[i], ZeroConst.PLATFORM_DIR_NAME);
            }

            localResDir = ZeroConst.WWW_RES_PERSISTENT_DATA_PATH;

            //确保本地资源目录存在
            if (false == Directory.Exists(localResDir))
            {
                Directory.CreateDirectory(localResDir);
            }
        }

        /// <summary>
        /// 获取启动参数，如果指定的参数不存在，则返回defaultValue
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue">key不存在时，返回改变量</param>
        /// <returns></returns>
        public static string GetStartupParams(string key, string defaultValue = null)
        {
            if (null == setting || null == setting.startupParams)
            {
                return defaultValue;
            }

            if (false == setting.startupParams.ContainsKey(key))
            {
                return defaultValue;
            }

            return setting.startupParams[key];
        }

        public static void PrintInfo()
        {
            Debug.Log(LogColor.Zero2($"[Zero][Runtime] ========================================="));
            Debug.Log(LogColor.Zero2($"[Zero][Runtime] 平台: {PlatformName}"));
            Debug.Log(LogColor.Zero2($"[Zero][Runtime] Scripting Backend: {(IsIL2CPP ? "IL2CPP" : "Mono")}"));
            Debug.Log(LogColor.Zero2($"[Zero][Runtime] HybridCLR: {(IsHybridClrEnable ? "启用" : "禁止")}"));
            Debug.Log(LogColor.Zero2($"[Zero][Runtime] 日志打印: {LauncherData.isLogEnable}"));
            Debug.Log(LogColor.Zero2($"[Zero][Runtime] 资源加载: {(LauncherData.isUseAssetBundle ? "AssetBundle" : "AssetDataBase")}"));
            if (LauncherData.isUseAssetBundle)
            {
                Debug.Log(LogColor.Zero2($"[Zero][Runtime] 热更功能: {LauncherData.isHotPatchEnable}"));
                if (LauncherData.isHotPatchEnable)
                {
                    if (null != SettingFileNetDirList && SettingFileNetDirList.Length > 0)
                    {
                        for (var i = 0; i < SettingFileNetDirList.Length; i++)
                        {
                            Debug.Log(LogColor.Zero2($"[Zero][Runtime] 网络资源Url({i})[{SettingFileNetDirList[i]}]"));
                        }
                    }
                    else
                    {
                        Debug.Log(LogColor.Zero2("[Zero][Runtime] 未配置网络资源Url"));
                    }
                }

                Debug.Log(LogColor.Zero2($"[Zero][Runtime] 允许离线运行: {LauncherData.isOfflineEnable}"));
            }

            Debug.Log(LogColor.Zero2($"[Zero][Runtime] 使用dll: {LauncherData.isUseDll}"));
            Debug.Log(LogColor.Zero2($"[Zero][Runtime] StreamingAssets资源路径: {ZeroConst.STREAMING_ASSETS_PATH}"));
            Debug.Log(LogColor.Zero2($"[Zero][Runtime] Persistent可读写目录路径: {ZeroConst.PERSISTENT_DATA_PATH}"));
            Debug.Log(LogColor.Zero2($"[Zero][Runtime] 下载资源存放路径: {localResDir}"));
            Debug.Log(LogColor.Zero2($"[Zero][Runtime] 框架生成文件存放路径: {generateFilesDir}"));
            if (setting.startupParams != null)
            {
                foreach (var kv in setting.startupParams)
                {
                    Debug.Log(LogColor.Zero2($"[Zero][Runtime] 启动参数: [{kv.Key}] => [{kv.Value}]"));
                }
            }

            Debug.Log(LogColor.Zero2($"[Zero][Runtime] ========================================="));
        }

        /// <summary>
        /// 执行一次内存回收(该接口开销大，可能引起卡顿)
        /// </summary>
        public static void GC()
        {
            //回收托管内存，确保Unity资源引用计数归零
            System.GC.Collect();
            //释放原生资源
            Resources.UnloadUnusedAssets();
        }
    }
}