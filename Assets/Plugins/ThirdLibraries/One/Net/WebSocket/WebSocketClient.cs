using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace One
{
    public class WebSocketClient
    {
        /// <summary>
        /// 协议是否已升级
        /// </summary>
        public bool IsUpgrade { get; internal set; } = false;

        /// <summary>
        /// TCP连接（WebSocket其实就是基于Tcp连接的)
        /// </summary>
        public WebSocketChannel Channel { get; private set; }

        /// <summary>
        /// 连接成功事件(多线程事件）
        /// </summary>
        public event Action<WebSocketClient> onConnectSuccess;

        /// <summary>
        /// 连接断开事件(多线程事件）
        /// </summary>
        public event Action<WebSocketClient> onDisconnect;

        /// <summary>
        /// 连接失败事件(多线程事件）
        /// </summary>
        public event Action<WebSocketClient> onConnectFail;

        /// <summary>
        /// 收到数据
        /// </summary>
        public event Action<WebSocketClient, byte[]> onReceiveData;

        /// <summary>
        /// 主机地址
        /// </summary>
        public string Host { get; private set; }

        /// <summary>
        /// 主机端口
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// 缓冲区大小
        /// </summary>
        public ushort BufferSize { get; private set; }

        /// <summary>
        /// 线程同步器，将异步方法同步到调用Refresh的线程中
        /// </summary>
        ThreadSyncActions _tsa = new ThreadSyncActions();

        /// <summary>
        /// 连接异步事件
        /// </summary>
        SocketAsyncEventArgs _connectEA;

        /// <summary>
        /// 是否已连接
        /// </summary>
        /// <returns></returns>
        public bool IsConnected
        {
            get
            {
                if (Channel != null && Channel.IsConnected && Channel.IsUpgrade)
                {
                    return true;
                }
                return false;
            }
        }

        public WebSocketClient()
        {
            
        }

        /// <summary>
        /// 连接指定的服务器
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="bufferSize"></param>
        public void Connect(string host, int port, ushort bufferSize)
        {
            Host = host;
            Port = port;
            BufferSize = bufferSize;

            Reconnect();
        }

        public void Reconnect()
        {
            Close(true);

            _tsa.Clear();
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(Host), Port);
            Socket socket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _connectEA = new SocketAsyncEventArgs();
            _connectEA.RemoteEndPoint = ipe;
            _connectEA.Completed += OnAsyncEventCompleted;
            if (!socket.ConnectAsync(_connectEA))
            {
                OnAsyncEventCompleted(null, _connectEA);
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public void Refresh()
        {
            _tsa.RunSyncActions();
            Channel?.Refresh();
        }

        /// <summary>
        /// 断开客户端连接
        /// </summary>
        public void Close(bool isSilently = false)
        {
            if (null != _connectEA)
            {
                _connectEA.Completed -= OnAsyncEventCompleted;
                _connectEA.Dispose();
                _connectEA = null;
            }

            if (null != Channel)
            {
                Channel.Close(isSilently);
                Channel = null;
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="bytes"></param>
        public virtual void Send(byte[] bytes)
        {
            if (Channel.IsUpgrade)
            {
                Channel.Send(bytes);
            }
        }

        /// <summary>
        /// 连接完成（多线程事件）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAsyncEventCompleted(object sender, SocketAsyncEventArgs e)
        {
            _tsa.AddToSyncAction(
                () =>
                {
                    OnConnectCompleted(e);
                });
        }

        void OnConnectCompleted(SocketAsyncEventArgs e)
        {
            e.Completed -= OnAsyncEventCompleted;
            if (null == e.ConnectSocket)
            {
                onConnectFail?.Invoke(this);
                return;
            }

            InitChannel(e.ConnectSocket, BufferSize);            
        }

        void InitChannel(Socket socket, int bufferSize)
        {
            Channel = new WebSocketChannel(socket, bufferSize);
            Channel.onReceiveData += OnReceiveData;
            Channel.onShutdown += OnShutdown;
            Channel.onUpgradeResult += OnUpgradeResult;
            Channel.RequestUpgrade();
        }

        private void OnUpgradeResult(WebSocketChannel channel, bool success)
        {
            if(true == success)
            {
                onConnectSuccess?.Invoke(this);
            }
            else
            {
                onConnectFail?.Invoke(this);
            }
        }

        private void OnReceiveData(IChannel sender, byte[] data)
        {
            onReceiveData?.Invoke(this, data);
        }

        private void OnShutdown(TcpChannel obj)
        {
            Channel = null;
            onDisconnect?.Invoke(this);
        }
    }
}
