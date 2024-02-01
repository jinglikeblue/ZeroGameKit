using Jing;
using One;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using UnityEngine;
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
                CreateMessageDispatcher();
                Debug.Log($"[创建HOST] IP:{SocketUtility.GetIPv4Address()}");
                _server.onClientEnter += OnClientEnter;
                _server.onClientExit += OnClientExit;
                _server.Start(PORT);
            }
        }

        /// <summary>
        /// 创建消息派发器
        /// </summary>
        void CreateMessageDispatcher()
        {                             
            var receiverInterfaceType = typeof(IMessageReceiver);

            MessageDispatcher<int> md = new MessageDispatcher<int>();            
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                if (false == type.IsAbstract)
                {
                    if (receiverInterfaceType.IsAssignableFrom(type))
                    {
                        var genericArguments = type.BaseType.GetGenericArguments();
                        if (genericArguments.Length == 1)
                        {
                            var protocolStruct = genericArguments[0];                            
                            if (protocolStruct.GetCustomAttribute(Protocols.ProtocolAttributeType) != null)
                            {
                                var id = Protocols.GetProtocolId(protocolStruct);
                                Debug.Log($"Found Receiver: [{id}] => {type.FullName}");
                                md.RegisterReceiver(id, type);
                            }
                        }
                    }
                }
            }
            _msgDispatcher = md;

            //测试Receiver
            //var body = new Protocols.GameStartNotify();
            //md.DispatchMessage(body.GetHashCode(), body);
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
    }
}
