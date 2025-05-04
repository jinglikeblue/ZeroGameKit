using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 程序启动器
    /// </summary>
    public class Launcher
    {
        public enum EState
        {
            /// <summary>
            /// 内嵌资源创始器
            /// </summary>
            STREAMING_ASSETS_RES_INITIATOR,

            /// <summary>
            /// 更新Setting.json
            /// </summary>
            SETTING_JSON_INITIATOR,

            /// <summary>
            /// 客户端更新
            /// </summary>
            APP_UPDATE_INITIATOR,

            /// <summary>
            /// 资源更新
            /// </summary>
            STARTUP_RES_INITIATOR,

            /// <summary>
            /// 启动主程序
            /// </summary>
            STARTUP
        }

        public LauncherSettingData launcherData;

        /// <summary>
        /// 当前状态
        /// </summary>
        public EState CurrentState { get; private set; }

        /// <summary>
        /// 状态改变的委托
        /// </summary>
        public event Action<EState> onStateChange;
        /// <summary>
        /// 状态对应进度的委托
        /// </summary>
        public event BaseInitiator.InitiatorProgress onProgress;

        /// <summary>
        /// Launcher启动失败
        /// </summary>
        public event Action<string> onError;

        StreamingAssetsResInitiator _streamingAssetsResInitiator;

        SettingJsonInitiator _settingJsonInitiator;

        AppUpdateInitiator _appUpdateInitiator;

        StartupResInitiator _startupResInitiator;

        /// <summary>
        /// 启动配置数据
        /// </summary>
        /// <param name="data">启动配置数据</param>
        public Launcher(LauncherSettingData data)
        {
            if (false == Application.isEditor)
            {
                //非Editor下，将会强制开启AssetBundle模式
                data.isUseAssetBundle = true;
            }
            
            launcherData = data;
        }

        /// <summary>
        /// 开始激活预加载
        /// </summary>
        /// <param name="rg"></param>
        public async void Start()
        {
            //实例化Native桥接器
            var startupNativeBridge = NativeBridge.Ins;

            //内嵌资源检查
            await new StreamingAssetsCheckInitiator().StartAsync();
            //初始化运行环境配置环境(初始化时依赖StramingAssets的资源)
            await Runtime.Ins.Init(launcherData);
            SettingJsonInit();
            // StreamingAssetsResInit();
        }

        #region StreamingAssetsResInit -> InitRuntime

        void StreamingAssetsResInit()
        {
            ChangeState(EState.STREAMING_ASSETS_RES_INITIATOR);
            _streamingAssetsResInitiator = new StreamingAssetsResInitiator();
            _streamingAssetsResInitiator.onComplete += OnStreamingAssetsResInitiatorComplete;
            _streamingAssetsResInitiator.Start();
        }

        private void OnStreamingAssetsResInitiatorComplete(BaseInitiator initiator)
        {
            _streamingAssetsResInitiator.onComplete -= OnStreamingAssetsResInitiatorComplete;

            Runtime.Ins.streamingAssetsResInitiator = _streamingAssetsResInitiator;

            if (initiator.error != null)
            {
                Error(initiator.error);
            }
            else
            {
                InitRuntime();
            }
        }

        #endregion

        void InitRuntime()
        {
            //初始化运行环境配置环境(初始化时依赖StramingAssets的资源)
            Runtime.Ins.Init(launcherData);

            SettingJsonInit();
        }

        #region SettingJsonInit -> AppClientInit

        void SettingJsonInit()
        {
            ChangeState(EState.SETTING_JSON_INITIATOR);
            _settingJsonInitiator = new SettingJsonInitiator();
            _settingJsonInitiator.onComplete += OnSettingJsonInitiatorComplete;
            _settingJsonInitiator.Start();
        }

        private void OnSettingJsonInitiatorComplete(BaseInitiator initiator)
        {
            _settingJsonInitiator.onComplete -= OnSettingJsonInitiatorComplete;

            if (initiator.error != null)
            {
                if (launcherData.isOfflineEnable)
                {
                    Debug.Log(Zero.LogColor.Zero1("[Launcher] 没有找到网络资源信息，将关闭热更功能，以离线模式运行"));
                    launcherData.isHotPatchEnable = false;
                }
                else
                {
                    Error(initiator.error);
                    return;
                }
            }

            AppClientInit();
        }

        #endregion

        #region AppClientInit -> StartupResInit

        /// <summary>
        /// 客户端更新
        /// </summary>
        void AppClientInit()
        {
            ChangeState(EState.APP_UPDATE_INITIATOR);
            _appUpdateInitiator = new AppUpdateInitiator();
            _appUpdateInitiator.onComplete += OnAppUpdateInitiatorComplete;
            _appUpdateInitiator.onProgress += OnAppUpdateInitiatorProgress;
            _appUpdateInitiator.Start();
        }

        private void OnAppUpdateInitiatorProgress(long loadedSize, long totalSize)
        {
            OnProgress(loadedSize, totalSize);
        }

        private void OnAppUpdateInitiatorComplete(BaseInitiator initiator)
        {
            _appUpdateInitiator.onComplete -= OnAppUpdateInitiatorComplete;
            _appUpdateInitiator.onProgress -= OnAppUpdateInitiatorProgress;

            if (initiator.error != null)
            {
                Error(initiator.error);
            }
            else
            {
                StartupResInit();
            }
        }

        #endregion

        #region StartupResInit -> ScriptsInit

        /// <summary>
        /// 更新初始化所需资源
        /// </summary>
        void StartupResInit()
        {
            ChangeState(EState.STARTUP_RES_INITIATOR);
            _startupResInitiator = new StartupResInitiator();
            _startupResInitiator.onComplete += OnStartupResInitiatorComplete;
            _startupResInitiator.onProgress += OnStartupResInitiatorProgress;
            _startupResInitiator.Start();
        }

        private void OnStartupResInitiatorProgress(long loadedSize, long totalSize)
        {
            OnProgress(loadedSize, totalSize);
        }

        private void OnStartupResInitiatorComplete(BaseInitiator initiator)
        {
            _startupResInitiator.onComplete -= OnAppUpdateInitiatorComplete;
            _startupResInitiator.onProgress -= OnAppUpdateInitiatorProgress;

            if (initiator.error != null)
            {
                Error(initiator.error);
            }
            else
            {
                ScriptsInit();
            }
        }

        #endregion

        void OnProgress(long loadedSize, long totalSize)
        {
            //Log.W("Progress: {0}", progress);
            onProgress?.Invoke(loadedSize, totalSize);
        }

        void ChangeState(EState state)
        {
            CurrentState = state;
            Debug.Log(LogColor.Zero1($"Launcher State: {state}"));
            onStateChange?.Invoke(state);
        }

        /// <summary>
        /// 发生错误
        /// </summary>
        /// <param name="error"></param>
        private void Error(string error)
        {
            onError?.Invoke(error);
        }

        void ScriptsInit()
        {
            ChangeState(EState.STARTUP);
            var scriptsInitiator = new ScriptsInitiator();
            scriptsInitiator.Start();
        }
    }
}