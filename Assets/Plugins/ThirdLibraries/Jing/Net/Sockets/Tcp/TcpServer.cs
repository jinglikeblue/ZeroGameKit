using Jing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Jing.Net
{
    /// <summary>
    /// 提供基于TCP协议的套接字服务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TcpServer : IServer
    {
        /// <summary>
        /// 检查死连接的间隔
        /// </summary>
        const int CHECK_DEAD_CHANNEL_INTERVAL = 10 * 60 * 1000;

        /// <summary>
        /// 新的客户端进入的事件
        /// </summary>
        public event ClientEnterEvent onClientEnter;

        /// <summary>
        /// 客户端退出的事件
        /// </summary>
        public event ClientExitEvent onClientExit;

        /// <summary>
        /// 存活的通道字典
        /// </summary>
        Dictionary<EndPoint, IChannel> _liveChannelsDict = new Dictionary<EndPoint, IChannel>();

        /// <summary>
        /// 监听的端口
        /// </summary>
        protected Socket _socket;

        /// <summary>
        /// 已连接的客户端总数
        /// </summary>
        public int ClientCount { get; private set; } = 0;

        /// <summary>
        /// 缓冲区大小
        /// </summary>
        int _bufferSize;

        /// <summary>
        /// 启动Socket服务
        /// </summary>        
        /// <param name="port">监听的端口</param>
        /// <param name="bufferSize">每一个连接的缓冲区大小</param>
        public void Start(int port, int bufferSize)
        {
            Log.I($"Tcp Server Start! Lisening {IPAddress.Any}:{port}");

            _bufferSize = bufferSize;
            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(new IPEndPoint(IPAddress.Any, port));
            _socket.Blocking = false;
            _socket.Listen(1000);
            AcceptLoop();
            CleanDeadChannelLoop();
        }

        /// <summary>
        /// 接受连接的异步循环
        /// </summary>
        async void AcceptLoop()
        {
            while (_socket != null)
            {
                var clientSocket = await _socket.AcceptAsync();
                //添加一个成功连接
                Enter(clientSocket);
            }
        }

        /// <summary>
        /// 关闭Socket服务
        /// </summary>
        public void Close()
        {
            if (null != _socket)
            {
                lock (_socket)
                {
                    try
                    {
                        _socket.Shutdown(SocketShutdown.Both);
                    }
                    catch
                    {
                    };
                    _socket.Close();
                }
                _socket = null;
            }
        }

        public void Refresh()
        {
            //异步编程模型下，不需要
        }

        void Enter(Socket clientSocket)
        {
            TcpChannel channel = new TcpChannel(clientSocket, _bufferSize);
            channel.onChannelClosed += OnClientShutdown;
            AddChannelToLiveDict(channel);
            onClientEnter?.Invoke(channel);
            Log.I($"新的连接，连接总数:{ClientCount}");
        }

        private void OnClientShutdown(IChannel channel)
        {
            channel.onChannelClosed -= OnClientShutdown;
            RemoveChannelFromLiveDict(channel);
            Log.I($"连接断开，连接总数:{ClientCount}");
        }

        /// <summary>
        /// 添加通道到活跃字典
        /// </summary>
        /// <param name="channel"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void AddChannelToLiveDict(IChannel channel)
        {
            lock (_liveChannelsDict)
            {
                _liveChannelsDict[channel!.RemoteEndPoint] = channel;
                ClientCount = _liveChannelsDict.Count;
            }
        }

        /// <summary>
        /// 移除活跃字典中，断开的通道
        /// </summary>
        /// <param name="channel"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void RemoveChannelFromLiveDict(IChannel channel)
        {
            lock (_liveChannelsDict)
            {
                //先添加到集合，稍后处理，现在处理则ChannelList会异常
                _liveChannelsDict.Remove(channel!.RemoteEndPoint);
                ClientCount = _liveChannelsDict.Count;
            }
        }

        async void CleanDeadChannelLoop()
        {
            while (_socket != null)
            {
                lock (_liveChannelsDict)
                {
                    var channelArr = _liveChannelsDict.Values.ToArray();
                    foreach (var c in channelArr)
                    {
                        if (!c.IsConnected)
                        {
                            _liveChannelsDict.Remove(c.RemoteEndPoint);
                        }
                    }
                    ClientCount = _liveChannelsDict.Count;
                }

                await Task.Delay(CHECK_DEAD_CHANNEL_INTERVAL);
            }
        }
    }
}
