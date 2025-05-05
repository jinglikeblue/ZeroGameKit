using Jing;
using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 「Zero」框架运行时
    /// </summary>
    public class Runtime
    {
        /// <summary>
        /// 构建单例
        /// </summary>
        public static readonly Runtime Ins = new Runtime();

        /// <summary>
        /// 启动器设置数据
        /// </summary>
        internal LauncherSettingData LauncherData;

        /// <summary>
        /// 内嵌资源初始化器
        /// </summary>
        internal readonly BuiltinInitiator BuiltinInitiator = new BuiltinInitiator();

        /// <summary>
        /// 资源模式是否仅使用包内资源（离线资源模式）
        /// </summary>
        public bool IsOnlyUseBuiltinRes => false == LauncherData.isHotPatchEnable;

        /// <summary>
        /// 是否允许离线运行
        /// </summary>
        public bool IsOfflineEnable => LauncherData.isOfflineEnable;

        /// <summary>
        /// 是否用DLL方式启动程序
        /// </summary>
        public bool IsUseDll => LauncherData.isUseDll;

        /// <summary>
        /// 本地数据
        /// </summary>
        public LocalDataModel localData { get; private set; }

        /// <summary>
        /// 配置
        /// </summary>
        public SettingVO setting;

        /// <summary>
        /// 网络资源目录列表
        /// </summary>
        public string[] SettingFileNetDirList { get; private set; }

        /// <summary>
        /// 基于运行平台的网络资源目录(使用的网络资源的地址)
        /// 举例: [资源所在URL地址]/res/[平台]
        /// </summary>
        public string netResDir;

        /// <summary>
        /// 资源对象版本数据
        /// </summary>
        public ResVerModel netResVer;

        /// <summary>
        /// 存放下载文件的目录
        /// 举例: [资源所在目录绝对路径]/res
        /// </summary>
        public string localResDir { get; private set; }

        /// <summary>
        /// 本地的资源版本
        /// </summary>
        public BaseWriteableResVerModel localResVer { get; private set; }

        /// <summary>
        /// Zero框架生成的文件的目录
        /// </summary>
        public string generateFilesDir { get; private set; }

        /// <summary>
        /// 是否使用AssetBundle加载资源
        /// </summary>
        public bool IsUseAssetBundle => LauncherData.isUseAssetBundle;

        /// <summary>
        /// 是否使用AssetDataBase加载资源
        /// </summary>
        public bool IsUseAssetDataBase => !LauncherData.isUseAssetBundle;

        /// <summary>
        /// 运行时是否依赖网络（需要更新资源）
        /// </summary>
        public bool IsNeedNetwork => LauncherData.isUseAssetBundle && LauncherData.isHotPatchEnable;

        /// <summary>
        /// 运行时是否允许加载热更目录中的资源 
        /// </summary>
        public bool IsHotResEnable => LauncherData.isUseAssetBundle && LauncherData.isHotPatchEnable;

        /// <summary>
        /// 设置是否打印日志
        /// </summary>
        /// <param name="enable"></param>
        internal void SetLogEnable(bool enable)
        {
            LauncherData.isLogEnable = enable;
            //日志控制
            Debug.unityLogger.logEnabled = enable;
        }

        internal void Init(LauncherSettingData data)
        {
            if (null != LauncherData)
            {
                Debug.LogError("Runtime无法重复初始化!");
                return;
            }

            this.LauncherData = data;

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
                    break;
                default:
                    throw new System.Exception(string.Format("抱歉！Zero暂时不支持平台：{0}", Application.platform));
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
        void InitHotResRuntime()
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
        public string GetStartupParams(string key, string defaultValue = null)
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

        public void PrintInfo()
        {
            Debug.Log(LogColor.Zero2($"[Zero][Runtime] ========================================="));
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
            Debug.Log(LogColor.Zero2($"[Zero][Runtime] ========================================="));
        }
    }
}