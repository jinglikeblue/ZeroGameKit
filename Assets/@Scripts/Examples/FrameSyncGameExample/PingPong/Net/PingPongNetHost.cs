using Jing;
using Jing.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using Example;
using UnityEngine;
using Zero;
using Zero;

namespace PingPong
{
    /// <summary>
    /// 主机
    /// </summary>
    public class PingPongNetHost
    {
        public const int PORT = 30101;

        /// <summary>
        /// UDP服务
        /// </summary>
        KcpServer _kcpServer;

        /// <summary>
        /// 是否活跃中
        /// </summary>
        public bool IsActive => _kcpServer == null ? false : true;

        /// <summary>
        /// 客户端连接通道
        /// </summary>
        IChannel _channel;

        /// <summary>
        /// 协议派发器
        /// </summary>
        MessageDispatcher<int> _msgDispatcher;
        
        /// <summary>
        /// 网络关闭事件
        /// </summary>
        public event Action onClose;

        /// <summary>
        /// 心跳
        /// </summary>
        public HeartbeatModel heartbeat => Global.Ins.netModule.heartbeat;
        
        /// <summary>
        /// 启动服务
        /// </summary>
        public void Start()
        {
            Protocols.Init();
            if (null == _kcpServer)
            {
                Debug.Log($"[创建HOST] IP:{SocketUtility.GetIPv4Address()}");
                _kcpServer = new KcpServer();
                _kcpServer.onClientEnter += OnClientEnter;
                _kcpServer.onClientExit += OnClientExit;
                _kcpServer.Start(PORT);
            }
        }
        
        void OnClientEnter(IChannel channel)
        {
            //一次只能接受一个连接
            if (null == _channel)
            {               
                _channel = channel;
                _channel.onReceivedData += OnReceiveData;
                heartbeat.Refresh();
            }
        }

        void OnClientExit(IChannel channel)
        {
            CloseChannel();
        }

        void OnReceiveData(IChannel sender, byte[] data)
        {
            Protocols.UnpackAndDispatch(data);
        }

        /// <summary>
        /// 关闭通道
        /// </summary>
        void CloseChannel()
        {
            if (null != _channel)
            {
                _channel.onReceivedData -= OnReceiveData;
                _channel.Close(true);
                _channel = null;
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop(bool isSliently = true)
        {
            CloseChannel();
            if (_kcpServer != null)
            {
                Debug.Log($"停止HOST");
                _kcpServer.onClientEnter -= OnClientEnter;
                _kcpServer.onClientExit -= OnClientExit;
                _kcpServer.Close();
                _kcpServer = null;

                if (!isSliently)
                {
                    onClose?.Invoke();
                }
            }
        }

        public void Update()
        {
            NetworkCheck();
        }
        
        /// <summary>
        /// 网络检查
        /// </summary>
        private void NetworkCheck()
        {
            if (null == _channel)
            {
                return;
            }
            
            if (heartbeat.IsPingReceivedTimeout)
            {
                Stop(false);
            }
        }

        public void SendProtocol(object protocolBody)
        {
            if (null == _kcpServer || null == _channel)
            {
                return;
            }
            
            var data = Protocols.Pack(protocolBody);
            _channel.Send(data);
        }

        #region  业务协议

        public void GameStart()
        {
            var body = new Protocols.GameStartNotify();
            SendProtocol(body);
        }

        public void FrameInput(int frame, Protocols.InputRequest[] inputs)
        {
            var body = new Protocols.FrameInputNotify();
            body.frame = frame;
            body.inputs = inputs;
            SendProtocol(body);
        }

        public void Pong(Protocols.PingC2S pingBody)
        {
            var body = new Protocols.PongS2C();
            body.clientUTC = pingBody.clientUTC;
            body.serverUTC = TimeUtility.NowUtcMilliseconds;
            SendProtocol(body);
            
            heartbeat.PongSent();
        }

        #endregion
    }
}
