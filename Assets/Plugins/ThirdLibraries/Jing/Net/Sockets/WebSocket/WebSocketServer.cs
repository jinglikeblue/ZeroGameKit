using Jing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Jing.Net
{
    /// <summary>
    /// 提供基于WebSocket协议的套接字服务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WebSocketServer:IServer
    {
        /// <summary>
        /// 新的客户端进入的事件
        /// </summary>
        public event ClientEnterEvent onClientEnter;

        /// <summary>
        /// 客户端退出的事件
        /// </summary>
        public event ClientExitEvent onClientExit;

        /// <summary>
        /// 线程同步器，将异步方法同步到调用Refresh的线程中
        /// </summary>
        ThreadSyncActions _tsa = new ThreadSyncActions();

        List<WebSocketChannel> _channelList = new List<WebSocketChannel>();

        /// <summary>
        /// 监听的端口
        /// </summary>
        protected Socket _socket;

        /// <summary>
        /// 断开的通道集合
        /// </summary>
        HashSet<WebSocketChannel> _shutdownSet = new HashSet<WebSocketChannel>();

        /// <summary>
        /// 已连接的客户端总数
        /// </summary>
        public int ClientCount
        {
            get
            {
                return _channelList.Count;
            }
        }

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
            Log.I($"Start Lisening {IPAddress.Any}:{port}");

            _bufferSize = bufferSize;
            _socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            try
            {
                _socket.Bind(new IPEndPoint(IPAddress.Any, port));
                _socket.Listen(100);
                StartAccept(null);
            }
            catch (Exception e)
            {
                Log.E(e.Message);
            }
        }

        /// <summary>
        /// 关闭Socket服务
        /// </summary>
        public void Close()
        {
            if (null != _socket)
            {
                //_socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                _socket = null;
            }
        }

        public void Refresh()
        {
            _tsa.RunSyncActions();

            //清理断开的通道
            RefreshShutdownSet();
        }

        /// <summary>
        /// 开始接受链接
        /// </summary>
        /// <param name="e"></param>
        void StartAccept(SocketAsyncEventArgs e)
        {
            if (e == null)
            {
                e = new SocketAsyncEventArgs();
                e.Completed += OnAcceptCompleted;
            }
            else
            {
                e.AcceptSocket = null;
            }

            bool willRaiseEvent = _socket.AcceptAsync(e);
            if (!willRaiseEvent)
            {
                ProcessAccept(e);
            }
        }

        /// <summary>
        /// 接收到连接完成的事件（多线程事件）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnAcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            _tsa.AddToSyncAction(() =>
            {
                ProcessAccept(e);
            });
        }

        void ProcessAccept(SocketAsyncEventArgs e)
        {
            //添加一个成功链接
            Enter(e.AcceptSocket);
            StartAccept(e);
        }

        void Enter(Socket clientSocket)
        {
            WebSocketChannel channel = new WebSocketChannel(clientSocket, _bufferSize);
            _channelList.Add(channel);
            Log.I($"新的连接，连接总数:{ClientCount}");

            channel.onChannelClosed += OnClientShutdown;

            if (channel.IsUpgrade)
            {
                OnUpgradeResult(channel, true);
            }
            else
            {
                channel.onUpgradeResult += OnUpgradeResult;
            }
        }

        private void OnUpgradeResult(WebSocketChannel channel, bool success)
        {
            channel.onUpgradeResult -= OnUpgradeResult;
            if (success)
            {
                onClientEnter?.Invoke(channel);
            }
            else
            {
                channel.Close();
            }
        }

        private void OnClientShutdown(IChannel channel)
        {
            channel.onChannelClosed -= OnClientShutdown;
            //先添加到集合，稍后处理，现在处理则ChannelList会异常
            _shutdownSet.Add(channel as WebSocketChannel);
        }

        void RefreshShutdownSet()
        {
            if (_shutdownSet.Count > 0)
            {
                foreach (var channel in _shutdownSet)
                {
                    _channelList.Remove(channel);
                    Log.I($"连接断开，连接总数:{ClientCount}");
                    if (channel.IsUpgrade)
                    {
                        onClientExit?.Invoke(channel);
                    }
                }
                _shutdownSet.Clear();
            }
        }
    }
}
