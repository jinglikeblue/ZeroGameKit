using Jing;
using System;
using System.Net;
using System.Runtime.CompilerServices;

namespace Jing.Net
{
    public class UdpClient
    {
        UdpListener _listener = null;
        UdpSendChannel _sendChannel = null;

        /// <summary>
        /// 收到UDP数据的事件（多线程事件）
        /// </summary>
        public event UdpClientReceivedDataEvent onReceivedData = null;

        public UdpClient()
        {
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Dispose()
        {
            _listener.Dispose();
            _sendChannel.Dispose();
            _listener = null;
            _sendChannel = null;
            onReceivedData = null;
        }

        /// <summary>
        /// 绑定Udp主机
        /// </summary>
        /// <param name="remoteHost">远程主机地址</param>
        /// <param name="remotePort">远程主机端口</param>
        /// <param name="localPort">本地监听端口</param>
        /// <param name="bufferSize">缓冲区大小</param>
        public void Bind(string remoteHost, int remotePort, int localPort, int bufferSize)
        {
            _listener = new UdpListener();
            _listener.onReceivedData += OnReceivedData;
            var socket = _listener.Bind(localPort, bufferSize);

            _sendChannel = new UdpSendChannel(socket, remoteHost, remotePort);
        }

        /// <summary>
        /// 绑定一个端口用来接收数据，但是没有对应的主机。用来广播数据时，采用此接口。
        /// </summary>
        /// <param name="localPort"></param>
        /// <param name="bufferSize"></param>
        public void Bind(int localPort, ushort bufferSize)
        {
            Bind(IPAddress.Any.ToString(), 0, localPort, bufferSize);
        }

        private void OnReceivedData(EndPoint remoteEndPoint, byte[] data)
        {
            onReceivedData?.Invoke(this, data);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="bytes"></param>
        public void Send(byte[] bytes)
        {
            _sendChannel?.Send(bytes);
        }

        /// <summary>
        /// 局域网数据广播
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="port">目标端口</param>
        public void Broadcast(byte[] bytes, int port)
        {
            _sendChannel?.SendTo(bytes, new IPEndPoint(IPAddress.Broadcast, port));
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
    }
}
