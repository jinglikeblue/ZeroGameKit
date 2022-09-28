using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// ����������
    /// </summary>
    public class Launcher
    {
        public enum EState
        {
            /// <summary>
            /// ��Ƕ��Դ��ʼ��
            /// </summary>
            STREAMING_ASSETS_RES_INITIATOR,

            /// <summary>
            /// ����Setting.json
            /// </summary>
            SETTING_JSON_INITIATOR,

            /// <summary>
            /// �ͻ��˸���
            /// </summary>
            APP_UPDATE_INITIATOR,

            /// <summary>
            /// ��Դ����
            /// </summary>
            STARTUP_RES_INITIATOR,

            /// <summary>
            /// ����������
            /// </summary>
            STARTUP
        }

        public LauncherSettingData launcherData;

        /// <summary>
        /// ��ǰ״̬
        /// </summary>
        public EState CurrentState { get; private set; }

        /// <summary>
        /// ״̬�ı��ί��
        /// </summary>
        public event Action<EState> onStateChange;
        /// <summary>
        /// ״̬��Ӧ���ȵ�ί��
        /// </summary>
        public event BaseInitiator.InitiatorProgress onProgress;

        /// <summary>
        /// Launcher����ʧ��
        /// </summary>
        public event Action<string> onError;

        StreamingAssetsResInitiator _streamingAssetsResInitiator;

        SettingJsonInitiator _settingJsonInitiator;

        AppUpdateInitiator _appUpdateInitiator;

        StartupResInitiator _startupResInitiator;

        bool _isAutoOffline = false;

        /// <summary>
        /// ������������
        /// </summary>
        /// <param name="data">������������</param>
        /// <param name="isAutoOffline">���Ȳ���ģʽ��ʱ��������ʲ���������Դ���Ƿ��Զ��л�Ϊ����ģʽ��ʹ����Ƕ��Դ�������У�</param>
        public Launcher(LauncherSettingData data, bool isAutoOffline = false)
        {
            this.launcherData = data;
            _isAutoOffline = isAutoOffline;
        }

        /// <summary>
        /// ��ʼ����Ԥ����
        /// </summary>
        /// <param name="rg"></param>
        public void Start()
        {
            //ʵ����Native�Ž���
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
            //��ʼ�����л������û���
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
                    Debug.Log(Zero.Log.Zero1("�Զ��л�Ϊ����ʹ����Ƕ��Դģʽ��"));
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
        /// �ͻ��˸���
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
        /// ���³�ʼ��������Դ
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
        /// ��������
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