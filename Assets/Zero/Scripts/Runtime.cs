using Jing;
using System;
using System.IO;
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
        internal LauncherSettingData VO;

        internal StreamingAssetsResInitiator streamingAssetsResInitiator;

        /// <summary>
        /// 内嵌资源模式
        /// </summary>
        public EBuiltinResMode BuiltinResMode => VO.builtinResMode;

        /// <summary>
        /// 资源模式是否仅使用包内资源（离线资源模式）
        /// </summary>
        public bool IsOnlyUseBuiltinRes
        {
            get
            {
                return BuiltinResMode == EBuiltinResMode.ONLY_USE ? true : false;
            }
        }

        /// <summary>
        /// 热更资源模式
        /// </summary>
        public EHotResMode HotResMode => VO.hotResMode;

        /// <summary>
        /// DLL执行模式
        /// </summary>
        public EILType ILType => VO.ilType;

        /// <summary>
        /// 是否用DLL方式启动程序
        /// </summary>
        public bool IsUseDll => VO.isUseDll;

        /// <summary>
        /// 是否加载PDB
        /// </summary>
        public bool IsLoadPdb => VO.isLoadPdb;

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
        /// 是否使用AssetDataBase加载资源
        /// </summary>
        public bool IsLoadAssetBundleByAssetDataBase
        {
            get
            {
                if (VO.hotResMode == EHotResMode.ASSET_DATA_BASE)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 运行时是否依赖网络（需要更新资源）
        /// </summary>
        public bool IsNeedNetwork
        {
            get
            {
                //正式发布的热更资源模式情况下，只要不是仅使用内嵌资源模式，都需要网络进行热更资源更新
                if (HotResMode == EHotResMode.NET_ASSET_BUNDLE)
                {
                    if (BuiltinResMode != EBuiltinResMode.ONLY_USE)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// 运行时是否允许加载热更目录中的资源 
        /// </summary>
        public bool IsHotResEnable
        {
            get
            {
                //正式发布的热更资源模式情况下，如果勾选了仅使用内嵌资源，则不允许从热更目录加载资源。否则都可以使用热更资源。
                if (HotResMode == EHotResMode.NET_ASSET_BUNDLE)
                {
                    if (BuiltinResMode == EBuiltinResMode.ONLY_USE)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// 内嵌资源是否存在
        /// </summary>
        public bool IsBuildinResExist
        {
            get
            {
                if (streamingAssetsResInitiator.IsResExist)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// 设置是否打印日志
        /// </summary>
        /// <param name="enable"></param>
        internal void SetLogEnable(bool enable)
        {
            VO.isLogEnable = enable;
            //日志控制
            Debug.unityLogger.logEnabled = enable;                       
        }


        internal void Init(LauncherSettingData vo)
        {
            this.VO = vo;

            SetLogEnable(vo.isLogEnable);

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
            Debug.Log(LogColor.Zero1($"[Runtime] GenerateFilesDir: {generateFilesDir}"));
            if (false == Directory.Exists(generateFilesDir))
            {
                Directory.CreateDirectory(generateFilesDir);
            }

            localData = new LocalDataModel();
            if (IsBuildinResExist)
            {
                localResVer = new LocalMixResVerModel(streamingAssetsResInitiator.resVerVO);
            }
            else
            {
                localResVer = new LocalResVerModel();
            }

            Debug.Log(LogColor.Zero1("[Runtime]Streaming Assets Dir: {0}", ZeroConst.STREAMING_ASSETS_PATH));

            if (null != SettingFileNetDirList)
            {
                for (var i = 0; i < SettingFileNetDirList.Length; i++)
                {
                    Debug.Log(LogColor.Zero1($"Net Res Root {i}        : {SettingFileNetDirList[i]}"));
                }
            }
            else
            {
                Debug.Log(LogColor.Zero1("Net Res Root        : Empty"));
            }
            Debug.Log(LogColor.Zero1("[Runtime]Persistent Data Dir : {0}", ZeroConst.PERSISTENT_DATA_PATH));
            Debug.Log(LogColor.Zero1("[Runtime]Local Res Dir       : {0}", localResDir == null ? "Empty" : localResDir));
            Debug.Log(LogColor.Zero1("[Runtime]Generate Files Dir  : {0}", generateFilesDir));


            CheckEnvironment();
        }

        /// <summary>
        /// 初始化热更资源相关的运行参数
        /// </summary>
        void InitHotResRuntime()
        {
            SettingFileNetDirList = new string[VO.netRoots.Length];
            for (var i = 0; i < SettingFileNetDirList.Length; i++)
            {
                SettingFileNetDirList[i] = FileUtility.CombineDirs(false, VO.netRoots[i], ZeroConst.PLATFORM_DIR_NAME);
            }

            switch (HotResMode)
            {
                case EHotResMode.NET_ASSET_BUNDLE:
                    localResDir = ZeroConst.WWW_RES_PERSISTENT_DATA_PATH;
                    break;
                default:
                    localResDir = ZeroConst.PUBLISH_RES_ROOT_DIR;
                    break;
            }


            //确保本地资源目录存在
            if (false == Directory.Exists(localResDir))
            {
                Directory.CreateDirectory(localResDir);
            }
        }

        /// <summary>
        /// 检查运行时环境
        /// </summary>
        void CheckEnvironment()
        {
            Debug.Log(LogColor.Zero1($"内嵌资源使用模式  : {BuiltinResMode}"));
            if (BuiltinResMode == EBuiltinResMode.ONLY_USE && false == streamingAssetsResInitiator.IsResExist)
            {
                throw new Exception($"[仅使用内嵌资源模式]下，{ZeroConst.STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW} 下的资源不正确");
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
    }
}