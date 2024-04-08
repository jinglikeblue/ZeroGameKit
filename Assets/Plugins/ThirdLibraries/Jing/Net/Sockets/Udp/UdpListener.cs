using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Jing.Net
{
    /// <summary>
    /// UDP协议监听器
    /// </summary>
    class UdpListener
    {
        /// <summary>
        /// 是否存活
        /// </summary>
        public bool IsAlive => _socket == null ? false : true;

        public Socket Socket => _socket;

        protected Socket _socket = null;

        protected byte[] _receiveBuffer = null;

        /// <summary>
        /// 收到UDP数据的事件
        /// </summary>
        public event UdpListenerReceivedDataEvent? onReceivedData;

        /// <summary>
        /// 监听的端口
        /// </summary>
        public int Port { get; private set; }

        EndPoint? _localEndPoint;

        public Socket Bind(int port, int bufferSize)
        {
            Port = port;
            _receiveBuffer = new byte[bufferSize];

            _localEndPoint = new IPEndPoint(IPAddress.Any, port);

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.Bind(_localEndPoint);

            //StartReceive();
            //ReceiveLoop();
            new Thread(ReceiveThread).Start();
            return _socket;
        }

        void ReceiveThread()
        {
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);            

            while (_socket != null)
            {                
                var receivedBytes = _socket.ReceiveFrom(_receiveBuffer, ref remoteEndPoint);
                if (0 == receivedBytes)
                {
                    break;
                }

                byte[] data = new byte[receivedBytes];
                Array.Copy(_receiveBuffer, data, receivedBytes);
                onReceivedData?.Invoke(remoteEndPoint, data);
            }

            Dispose();
        }

        async void ReceiveLoop()
        {

            while (_socket != null)
            {
                var result = await _socket.ReceiveFromAsync(new ArraySegment<byte>(_receiveBuffer), SocketFlags.None, _localEndPoint);
                if (0 == result.ReceivedBytes)
                {
                    break;
                }

                byte[] data = new byte[result.ReceivedBytes];
                Array.Copy(_receiveBuffer, data, result.ReceivedBytes);
                onReceivedData?.Invoke(result.RemoteEndPoint, data);
            }

            Dispose();
        }

        /// <summary>
        /// 销毁监听
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Dispose()
        {
            if (_socket != null)
            {
                try
                {
                    _socket.Shutdown(SocketShutdown.Receive);
                }
                catch
                {
                }
                _socket.Close();
                _socket.Dispose();

                _socket = null;
            }

            onReceivedData = null;
            _receiveBuffer = null;
        }
    }
}
