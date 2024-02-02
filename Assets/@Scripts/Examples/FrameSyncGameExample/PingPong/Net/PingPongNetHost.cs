using Jing;
using One;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
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
        KcpServer _server;

        /// <summary>
        /// 客户端连接通道
        /// </summary>
        IChannel _channel;

        /// <summary>
        /// 协议派发器
        /// </summary>
        MessageDispatcher<int> _msgDispatcher;

        /// <summary>
        /// 启动服务
        /// </summary>
        public void Start()
        {
            if (null == _server)
            {
                Debug.Log($"[创建HOST] IP:{SocketUtility.GetIPv4Address()}");
                _server = new KcpServer();
                _server.onClientEnter += OnClientEnter;
                _server.onClientExit += OnClientExit;
                _server.Start(PORT);
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
            var obj = Protocols.Unpack(data);
            //TODO 派发这个协议
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
        public void Stop()
        {
            CloseChannel();
            if (_server != null)
            {
                Debug.Log($"[停止HOST]");
                _server.onClientEnter -= OnClientEnter;
                _server.onClientExit -= OnClientExit;
                _server.Close();
                _server = null;
            }
        }

        public void Update()
        {
            _server.Refresh();            
        }

        public void SendProtocol(object protocolBody)
        {
            if (null == _server || null == _channel)
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
        }

        #endregion
    }
}
