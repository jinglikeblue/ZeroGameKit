using Jing;
using System;
using System.Net;
using System.Net.Sockets;

namespace Jing.Net
{
    public class UdpServer
    {
        UdpListener _listener;

        /// <summary>
        /// 收到UDP数据的事件
        /// </summary>
        public event UdpServerReceivedDataEvent onReceivedData;

        /// <summary>
        /// 线程同步器，将异步方法同步到调用Refresh的线程中
        /// </summary>
        ThreadSyncActions _tsa = new ThreadSyncActions();        

        /// <summary>
        /// 刷新网络，如果网络线程中有收到数据，会触发onReceivedData事件回调数据。
        /// </summary>
        public void Refresh()
        {            
            _tsa.RunSyncActions();
        }

        public void Dispose()
        {
            _tsa.Clear();
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
        public void Bind(int localPort, ushort bufferSize)
        {
            Log.I($"Bind Udp Lisening {IPAddress.Any}:{localPort}");           

            _listener = new UdpListener();
            _listener.onReceivedData += OnReceivedData;
            _listener.Bind(localPort, bufferSize, _tsa);
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
            var channel = new UdpSendChannel(_listener.Socket, remoteEndPoint, _tsa);
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
    }
}
