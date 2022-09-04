using System;
using System.Net;
using System.Net.Sockets;

namespace One
{
    public class UdpServer
    {
        UdpListener _listener;

        /// <summary>
        /// 收到UDP数据的事件
        /// </summary>
        public event Action<UdpServer, EndPoint, byte[]> onReceiveData;

        /// <summary>
        /// 线程同步器，将异步方法同步到调用Refresh的线程中
        /// </summary>
        ThreadSyncActions _tsa = new ThreadSyncActions();        

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
            onReceiveData = null;            
        }

        /// <summary>
        /// 启动Socket服务
        /// </summary>
        /// <param name="host">监听的地址</param>
        /// <param name="bindPort">坚挺的端口</param>
        /// <param name="bufferSize">每一个连接的缓冲区大小</param>
        public void Bind(int localPort, ushort bufferSize)
        {
            Log.CI(ConsoleColor.DarkGreen, "Bind Udp Lisening {0}:{1}", IPAddress.Any, localPort);           

            _listener = new UdpListener();
            _listener.onReceiveData += OnReceiveData;
            _listener.Bind(localPort, bufferSize, _tsa);
        }

        private void OnReceiveData(EndPoint remoteEndPoint, byte[] data)
        {
            onReceiveData?.Invoke(this, remoteEndPoint, data);
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
    }
}
