using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
            /// 内嵌资源创始器BuiltinInitiator
            /// </summary>
            BuiltinInitiator,

            /// <summary>
            /// 更新Setting.json
            /// </summary>
            SettingJsonInitiator,

            /// <summary>
            /// 客户端更新
            /// </summary>
            AppUpdateInitiator,

            /// <summary>
            /// 资源更新
            /// </summary>
            StartupResInitiator,

            /// <summary>
            /// 启动主程序
            /// </summary>
            ScriptsInitiator,

            /// <summary>
            /// 结束
            /// </summary>
            Finished
        }

        public readonly LauncherSettingData launcherData;

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
                
                if (WebGL.IsEnvironmentWebGL)
                {
                    //TODO 思考！如果勾选了热更功能，是否资源的预载应该通过Web服务来实现，方便测试。
                    
                    //如果是WebGL环境，则强制关闭热更功能以及使用DLL。WebGL下，Builtin资源就是热更资源。
                    data.isHotPatchEnable = false;
                    data.isUseDll = false;
                }
            }

            launcherData = data;
        }

        /// <summary>
        /// 开始激活预加载
        /// </summary>
        /// <param name="rg"></param>
        public async UniTask<string> Start()
        {
            string error = null;

            do
            {
                //实例化Native桥接器
                NativeBridge.Ins.Empty();

                //内嵌资源检查
                ChangeState(EState.BuiltinInitiator);
                error = await Runtime.BuiltinInitiator.StartAsync();
                if (error != null) break;

                //初始化运行环境配置环境(初始化时依赖StramingAssets的资源)
                Runtime.Init(launcherData);

                ChangeState(EState.SettingJsonInitiator);
                error = await new SettingJsonInitiator().StartAsync();
                if (error != null) break;

                ChangeState(EState.AppUpdateInitiator);
                var appUpdateInitiator = new AppUpdateInitiator();
                error = await appUpdateInitiator.StartAsync(OnProgress);
                if (error != null) break;

                ChangeState(EState.StartupResInitiator);
                var startupResInitiator = new StartupResInitiator();
                error = await startupResInitiator.StartAsync(OnProgress);
                if (error != null) break;

                ChangeState(EState.ScriptsInitiator);
                var scriptsInitiator = new ScriptsInitiator();
                error = await scriptsInitiator.StartAsync();
                if (error != null) break;

                Runtime.PrintInfo();
                ChangeState(EState.Finished);
            } while (false);

            if (error != null)
            {
                Error(error);
            }

            return error;
        }

        void OnProgress(long loadedSize, long totalSize)
        {
            onProgress?.Invoke(loadedSize, totalSize);
        }

        void ChangeState(EState state)
        {
            CurrentState = state;
            Debug.Log(LogColor.Zero1($"[Zero][Launcher] State: {state}"));
            onStateChange?.Invoke(state);
        }

        /// <summary>
        /// 发生错误
        /// </summary>
        /// <param name="error"></param>
        private void Error(string error)
        {
            Debug.LogError($"[Launcher] 启动失败: {error}");
            onError?.Invoke(error);
        }
    }
}