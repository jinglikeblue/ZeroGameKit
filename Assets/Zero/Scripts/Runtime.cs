﻿using Jing;
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
        /// RuntimeVO数据对象
        /// </summary>
        public RuntimeVO VO { get; private set; }

        /// <summary>
        /// 资源模式
        /// </summary>
        public EHotResMode ResMode
        {
            get { return VO.hotResMode; }
        }

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
        /// </summary>
        public string netResDir;

        /// <summary>
        /// 资源对象版本数据
        /// </summary>
        public ResVerModel netResVer;

        /// <summary>
        /// 存放下载文件的目录
        /// </summary>
        public string localResDir { get; private set; }

        /// <summary>
        /// 本地的资源版本
        /// </summary>
        public LocalResVerModel localResVer { get; private set; }

        /// <summary>
        /// Zero框架生成的文件的目录
        /// </summary>
        public string generateFilesDir { get; private set; }

        /// <summary>
        /// 是否使用AssetDataBase加载资源
        /// </summary>
        public bool IsLoadAssetsByAssetDataBase
        {
            get
            {
                if (VO.isHotResProject && VO.hotResMode == EHotResMode.ASSET_DATA_BASE)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 是否是从网络加载资源
        /// </summary>
        public bool IsLoadAssetsFromNet
        {
            get
            {
                if (VO.hotResMode == EHotResMode.NET_ASSET_BUNDLE)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 是否是热更资源项目
        /// </summary>
        public bool IsHotResProject
        {
            get
            {
                return VO.isHotResProject;
            }
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

            if (IsLoadAssetsFromNet)
            {
                localResDir = ZeroConst.WWW_RES_PERSISTENT_DATA_PATH;
            }
            else
            {
                localResDir = ZeroConst.PUBLISH_RES_ROOT_DIR;
            }

            //确保本地资源目录存在
            if (false == Directory.Exists(localResDir))
            {
                Directory.CreateDirectory(localResDir);
            }
        }

        public void Init(RuntimeVO vo)
        {
            this.VO = vo;

            //日志控制
            Log.IsActive = vo.isLogEnable;

            if (vo.isHotResProject)
            {
                InitHotResRuntime();
            }

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
            localResVer = new LocalResVerModel();

            Debug.Log(Log.Zero1("Streaming Assets Dir: {0}", ZeroConst.STREAMING_ASSETS_PATH));

            if (null != SettingFileNetDirList)
            {
                Debug.Log(Log.Zero1("Net Res Root        : "));
                for (var i = 0; i < SettingFileNetDirList.Length; i++)
                {
                    Debug.Log(Log.Zero1($"{i} : {SettingFileNetDirList[i]}"));
                }
            }
            else
            {
                Debug.Log(Log.Zero1("Net Res Root        : Empty"));
            }
            Debug.Log(Log.Zero1("Persistent Data Dir : {0}", ZeroConst.PERSISTENT_DATA_PATH));
            Debug.Log(Log.Zero1("Local Res Dir       : {0}", localResDir == null ? "Empty" : localResDir));
            Debug.Log(Log.Zero1("Generate Files Dir  : {0}", generateFilesDir));
        }
    }
}