using System;
using System.Net;

namespace One
{
    public class UdpClient
    {
        UdpListener _listener;
        UdpSendChannel _sendChannel;

        /// <summary>
        /// 收到UDP数据的事件（多线程事件）
        /// </summary>
        public event Action<UdpClient, byte[]> onReceiveData;

        /// <summary>
        /// 线程同步器，将异步方法同步到调用Refresh的线程中
        /// </summary>
        ThreadSyncActions _tsa = new ThreadSyncActions();

        public UdpClient()
        {

        }

        public void Dispose()
        {
            _tsa.Clear();
            _listener.Dispose();
            _sendChannel.Dispose();
            _listener = null;
            _sendChannel = null;
            onReceiveData = null;
        }

        /// <summary>
        /// 绑定Udp主机
        /// </summary>
        /// <param name="remoteHost">远程主机地址</param>
        /// <param name="remotePort">远程主机端口</param>
        /// <param name="localPort">本地监听端口</param>
        /// <param name="bufferSize">缓冲区大小</param>
        public void Bind(string remoteHost, int remotePort, int localPort, ushort bufferSize)
        {
            _listener = new UdpListener();
            _listener.onReceiveData += OnReceiveData;
            var socket = _listener.Bind(localPort, bufferSize, _tsa);

            _sendChannel = new UdpSendChannel(socket, remoteHost, remotePort, _tsa);
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

        private void OnReceiveData(EndPoint remoteEndPoint, byte[] data)
        {
            onReceiveData?.Invoke(this, data);
        }

        public void Refresh()
        {
            _tsa.RunSyncActions();
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="bytes"></param>
        public void Send(byte[] bytes)
        {
            _sendChannel.Send(bytes);
        }

        /// <summary>
        /// 局域网数据广播
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="port">目标端口</param>
        public void Broadcast(byte[] bytes, int port)
        {
            _sendChannel.SendTo(bytes, new IPEndPoint(IPAddress.Broadcast, port));
        }

        /// <summary>
        /// 局域网数据广播
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="ports">目标端口集合</param>
        public void Broadcast(byte[] bytes, int[] ports)
        {
            foreach(var port in ports)
            {
                Broadcast(bytes, port);
            }
        }
    }
}
