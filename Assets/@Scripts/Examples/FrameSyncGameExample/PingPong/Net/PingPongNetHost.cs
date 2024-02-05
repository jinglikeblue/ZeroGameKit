using Jing;
using One;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using Example;
using UnityEngine;
using Zero;
using ZeroHot;

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
        /// 启动服务
        /// </summary>
        public void Start()
        {
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
            }
        }

        void OnClientExit(IChannel channel)
        {
            if (null != _channel)
            {
                _channel.onReceivedData -= OnReceiveData;
                _channel.Close(true);
                _channel = null;
            }
        }

        void OnReceiveData(IChannel sender, byte[] data)
        {
            var md5 = MD5Helper.GetShortMD5(new MemoryStream(data), true);
            Debug.Log($"收到协议 [size:{data.Length}] [md5:{md5}]");
            Protocols.UnpackAndDispatch(data);
        }

        /// <summary>
        /// 关闭通道
        /// </summary>
        void CloseChannel()
        {
            _channel?.Close();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop(bool isSliently = true)
        {
            CloseChannel();
            if (_kcpServer != null)
            {
                Debug.Log($"[停止HOST]");
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
            _kcpServer?.Refresh();  
            
            NetworkCheck();
        }
        
        /// <summary>
        /// 网络检查
        /// </summary>
        private void NetworkCheck()
        {
            var idleTime = TimeUtility.NowUtcMilliseconds - Global.Ins.netModule.lastReceivePingPongUTC;
            
            if (idleTime > 10000)
            {
                //超过10秒没有收到消息，网络断开
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
            var md5 = MD5Helper.GetShortMD5(new MemoryStream(data), true);
            Debug.Log($"发送协议 [size:{data.Length}] [md5:{md5}]");
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
        }

        #endregion
    }
}
