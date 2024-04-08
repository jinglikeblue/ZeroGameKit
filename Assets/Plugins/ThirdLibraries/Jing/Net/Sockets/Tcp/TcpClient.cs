using Jing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Jing.Net
{
    public class TcpClient : IClient
    {
        /// <summary>
        /// 连接成功事件(多线程事件）
        /// </summary>
        public event ConnectServerSuccessEvent onConnectSuccess;

        /// <summary>
        /// 连接断开事件(多线程事件）
        /// </summary>
        public event DisconnectedEvent onDisconnected;

        /// <summary>
        /// 连接失败事件(多线程事件）
        /// </summary>
        public event ConnectServerFailEvent onConnectFail;

        /// <summary>
        /// 收到数据
        /// </summary>
        public event ReceivedServerDataEvent onReceivedData;

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
        public int BufferSize { get; private set; }

        /// <summary>
        /// 通信通道
        /// </summary>
        public TcpChannel Channel { get; private set; }

        /// <summary>
        /// 是否已连接
        /// </summary>
        /// <returns></returns>
        public bool IsConnected
        {
            get
            {
                if (Channel != null && Channel.IsConnected)
                {
                    return true;
                }
                return false;
            }
        }

        public TcpClient()
        {

        }

        /// <summary>
        /// 连接指定的服务器
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="bufferSize"></param>
        public void Connect(string host, int port, int bufferSize)
        {
            Host = host;
            Port = port;
            BufferSize = bufferSize;

            Reconnect();
        }

        public void Reconnect()
        {
            Close(true);
            ConnectAsync();
        }

        /// <summary>
        /// 异步连接
        /// </summary>
        async void ConnectAsync()
        {
            try
            {
                IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(Host), Port);
                Socket socket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                //等待连接
                await socket.ConnectAsync(ipe);

                InitChannel(socket, BufferSize);
                onConnectSuccess?.Invoke(this);
            }
            catch (SocketException ex)
            {
                // 捕获连接异常
                if (ex.SocketErrorCode == SocketError.ConnectionRefused)
                {
                    Log.I("连接被拒绝");
                }
                else if (ex.SocketErrorCode == SocketError.HostNotFound)
                {
                    Log.I("找不到指定的主机");
                }
                else
                {
                    Log.I("连接失败： " + ex.Message);
                }

                onConnectFail?.Invoke(this);
            }
        }

        /// <summary>
        /// 断开客户端连接
        /// </summary>
        /// <param name="isSilently">如果为true，则不会触发任何事件</param>
        public void Close(bool isSilently = false)
        {
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
            Channel?.Send(bytes);
        }

        void InitChannel(Socket socket, int bufferSize)
        {
            Channel = new TcpChannel(socket, bufferSize);
            Channel.onReceivedData += OnReceivedData;
            Channel.onChannelClosed += OnShutdown;
        }

        private void OnReceivedData(IChannel sender, byte[] data)
        {
            onReceivedData?.Invoke(this, data);
        }

        private void OnShutdown(IChannel channel)
        {
            Channel = null;
            onDisconnected?.Invoke(this);
        }
    }
}
