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

        bool _isAutoOffline = false;

        /// <summary>
        /// 启动配置数据
        /// </summary>
        /// <param name="data">启动配置数据</param>
        /// <param name="isAutoOffline">「热补丁模式」时，如果访问不到网络资源，是否自动切换为离线模式（使用内嵌资源继续运行）</param>
        public Launcher(LauncherSettingData data, bool isAutoOffline = false)
        {
            this.launcherData = data;
            _isAutoOffline = isAutoOffline;
        }

        /// <summary>
        /// 开始激活预加载
        /// </summary>
        /// <param name="rg"></param>
        public void Start()
        {
            //实例化Native桥接器
            var startupNativeBridge = NativeBridge.Ins;



            StreamingAssetsResInit();
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
            //初始化运行环境配置环境
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
                if (_isAutoOffline && EBuiltinResMode.HOT_PATCH == launcherData.builtinResMode)
                {
                    Debug.Log(Zero.Log.Zero1("自动切换为「仅使用内嵌资源模式」"));
                    launcherData.builtinResMode = EBuiltinResMode.ONLY_USE;
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
            Debug.Log(Log.Zero1($"Launcher State: {state}"));
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
            new ScriptsInitiator().Start();
        }
    }
}