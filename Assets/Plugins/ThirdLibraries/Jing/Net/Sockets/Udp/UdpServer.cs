using Jing;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace Jing.Net
{
    public class UdpServer : IServer
    {
        UdpListener? _listener;

        public bool IsAlive => _listener == null ? false : true;

        /// <summary>
        /// 收到UDP数据的事件
        /// </summary>
        public event UdpServerReceivedDataEvent? onReceivedData;

        public event ClientEnterEvent? onClientEnter;
        public event ClientExitEvent? onClientExit;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Dispose()
        {
            if (_listener != null)
            {
                _listener.Dispose();
                _listener = null;
            }

            onReceivedData = null;
        }

        /// <summary>
        /// 启动Socket服务
        /// </summary>        
        /// <param name="localPort">监听的端口</param>
        /// <param name="bufferSize">每一个连接的缓冲区大小</param>
        public void Bind(int localPort, int bufferSize)
        {
            Log.I($"Lisening UDP Port: {IPAddress.Any}:{localPort}");

            _listener = new UdpListener();
            _listener.onReceivedData += OnReceivedData;
            _listener.Bind(localPort, bufferSize);
        }

        private void OnReceivedData(EndPoint remoteEndPoint, byte[] data)
        {
            onReceivedData?.Invoke(this, remoteEndPoint, data);
        }

        /// <summary>
        /// 创建一个信息发送通道
        /// </summary>
        /// <param name="remoteEndPoint"></param>
        /// <returns></returns>
        public UdpSendChannel CreateSendChannel(EndPoint remoteEndPoint)
        {
            var channel = new UdpSendChannel(_listener.Socket, remoteEndPoint);
            return channel;
        }

        /// <summary>
        /// 局域网数据广播
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="port">目标端口</param>
        public void Broadcast(byte[] bytes, int port)
        {
            _listener.Socket.SendTo(bytes, new IPEndPoint(IPAddress.Broadcast, port));
        }

        /// <summary>
        /// 局域网数据广播
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="ports">目标端口集合</param>
        public void Broadcast(byte[] bytes, int[] ports)
        {
            foreach (var port in ports)
            {
                Broadcast(bytes, port);
            }
        }

        public void Start(int port, int bufferSize)
        {
            Bind(port, (ushort)bufferSize);
        }

        public void Close()
        {
            Dispose();
        }
    }
}