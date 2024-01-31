using Jing;
using One;
using System;
using System.Net;
using UnityEngine;

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
        /// 启动服务
        /// </summary>
        public void Start()
        {
            if (null == _server)
            {
                Debug.Log($"[创建HOST] IP:{SocketUtility.GetIPv4Address()}");
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

        }

        void OnReceiveData(IChannel sender, byte[] data)
        {
            
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
    }
}
