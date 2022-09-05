using System;
using System.Net;
using System.Net.Sockets;

namespace One
{
    /// <summary>
    /// UDP协议监听器
    /// </summary>
    class UdpListener
    {
        SocketAsyncEventArgs _receiveEA;

        public Socket Socket
        {
            get
            {
                return _socket;
            }
        }

        protected Socket _socket;

        protected byte[] _receiveBuffer;

        /// <summary>
        /// 收到UDP数据的事件
        /// </summary>
        public event UdpListenerReceiveDataEvent onReceiveData;

        /// <summary>
        /// 监听的端口
        /// </summary>
        public int Port { get; private set; }

        IPEndPoint _localEndPoint;

        /// <summary>
        /// 线程同步器
        /// </summary>
        ThreadSyncActions _tsa;

        public UdpListener()
        {
           
        }

        public Socket Bind(int port, ushort bufferSize, ThreadSyncActions tsa)
        {           
            Port = port;
            _receiveBuffer = new byte[bufferSize];
            _tsa = tsa;

            _localEndPoint = new IPEndPoint(IPAddress.Any, port);

            _receiveEA = new SocketAsyncEventArgs();
            _receiveEA.Completed += OnAsyncEventCompleted;                        
            _receiveEA.RemoteEndPoint = _localEndPoint;

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.Bind(_localEndPoint);

            StartReceive();

            return _socket;
        }

        /// <summary>
        /// 开始接受数据
        /// </summary>
        protected void StartReceive()
        {
            _receiveEA.SetBuffer(_receiveBuffer, 0, _receiveBuffer.Length);

            if (!_socket.ReceiveFromAsync(_receiveEA))
            {
                ProcessReceive(_receiveEA);
            }
        }

        /// <summary>
        /// 销毁监听
        /// </summary>
        public void Dispose()
        {
            if (null != _receiveEA)
            {
                _receiveEA.Completed -= OnAsyncEventCompleted;
                _receiveEA = null;
            }

            if (_socket != null)
            {
                _socket.Close();
                _socket.Dispose();
                try
                {                    
                    _socket.Shutdown(SocketShutdown.Both);
                }
                catch
                {
                }
                _socket = null;
            }

            onReceiveData = null;
            _receiveBuffer = null;
            _tsa = null;
        }

        /// <summary>
        /// 异步事件完成（多线程事件）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAsyncEventCompleted(object sender, SocketAsyncEventArgs e)
        {
            _tsa.AddToSyncAction(() => {
                ProcessReceive(e);
            });
        }


        /// <summary>
        /// 处理接收到的消息
        /// </summary>        
        void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {                
                byte[] data = new byte[e.BytesTransferred];
                Array.Copy(e.Buffer, data, e.BytesTransferred);
                onReceiveData?.Invoke(e.RemoteEndPoint, data);
                StartReceive();
            }
            else
            {
                //Dispose();
            }
        }
    }
}
