using Jing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace Jing.Net
{
    /// <summary>
    /// UDP协议发送器
    /// </summary>
    public class UdpSendChannel
    {
        protected Socket _socket = null;

        public EndPoint RemoteEndPoint { get; private set; } = null;

        internal UdpSendChannel(Socket socket, string remoteHost, int remotePort)
        {
            var remoteEndPoint = new IPEndPoint(IPAddress.Parse(remoteHost), remotePort);
            Init(socket, remoteEndPoint);
        }

        internal UdpSendChannel(Socket socket, EndPoint remoteEndPoint)
        {
            Init(socket, remoteEndPoint);
        }

        void Init(Socket socket, EndPoint remoteEndPoint)
        {
            _socket = socket;
            RemoteEndPoint = remoteEndPoint;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Dispose()
        {
            _socket = null;
            RemoteEndPoint = null;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="bytes"></param>
        public void Send(byte[] bytes)
        {
            _socket?.SendTo(bytes, RemoteEndPoint);
        }

        public void SendTo(byte[] bytes, IPEndPoint endPoint)
        {
            _socket.SendTo(bytes, endPoint);
        }
    }
}
