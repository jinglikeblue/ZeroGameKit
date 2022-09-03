using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace One
{
    public class TcpChannel : IChannel
    {
        SocketAsyncEventArgs _receiveEA;

        SocketAsyncEventArgs _sendEA;

        /// <summary>
        /// 线程同步器，将异步方法同步到调用Refresh的线程中
        /// </summary>
        ThreadSyncActions _tsa = new ThreadSyncActions();

        /// <summary>
        /// 收到数据
        /// </summary>
        public event ReceiveDataEvent onReceiveData;

        /// <summary>
        /// 客户端连接关闭事件
        /// </summary>
        internal event Action<TcpChannel> onShutdown;

        protected Socket _socket;

        protected byte[] _buffer;

        /// <summary>
        /// 数据发送队列
        /// </summary>
        protected List<ArraySegment<byte>> _sendBufferList = new List<ArraySegment<byte>>();

        /// <summary>
        /// 是否正在发送数据
        /// </summary>
        bool _isSending = false;

        /// <summary>
        /// 缓冲区可用字节长度
        /// </summary>
        protected int _bufferAvailable = 0;

        /// <summary>
        /// 协议处理器
        /// </summary>
        TcpProtocolProcess _protocolProcess;

        /// <summary>
        /// 是否客户端连接中
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (_socket != null && _socket.Connected)
                {
                    return true;
                }
                return false;
            }
        }

        public TcpChannel(Socket socket, int bufferSize)
        {
            _socket = socket;
            _buffer = new byte[bufferSize];

            CreateProtocolProcess();
            _receiveEA = new SocketAsyncEventArgs();
            _sendEA = new SocketAsyncEventArgs();
            _receiveEA.Completed += OnAsyncEventCompleted;
            _sendEA.Completed += OnAsyncEventCompleted;

            StartReceive();
        }       
        
        virtual protected void CreateProtocolProcess()
        {
            _protocolProcess = new TcpProtocolProcess();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        internal void Refresh()
        {
            _tsa.RunSyncActions();
        }

        /// <summary>
        /// 异步事件完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAsyncEventCompleted(object sender, SocketAsyncEventArgs e)
        {
            _tsa.AddToSyncAction(() =>
            {
                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.Receive:
                        ProcessReceive(e);
                        break;
                    case SocketAsyncOperation.Send:
                        ProcessSend(e);
                        break;
                    default:
                        throw new ArgumentException(string.Format("Wrong last operation : {0}", e.LastOperation));
                }
            });
        }

        protected void StartReceive()
        {
            if (false == IsConnected)
            {
                return;
            }

            _receiveEA.SetBuffer(_buffer, _bufferAvailable, _buffer.Length - _bufferAvailable);

            bool willRaiseEvent = _socket.ReceiveAsync(_receiveEA);
            if (!willRaiseEvent)
            {
                ProcessReceive(_receiveEA);
            }
        }

        /// <summary>
        /// 处理接收到的消息（多线程事件）
        /// </summary>
        /// <param name="e"></param>
        protected void ProcessReceive(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                _bufferAvailable += e.BytesTransferred;

                //协议处理器处理协议数据
                int used = UnpackProtocolData();

                if(false == IsConnected)
                {
                    //处理协议中导致通道关闭了
                    return;
                }

                if (used > 0)
                {
                    _bufferAvailable = _bufferAvailable - used;
                    if (0 != _bufferAvailable)
                    {
                        //将还没有使用的数据移动到数据开头
                        byte[] newBytes = new byte[_buffer.Length];
                        Array.Copy(_buffer, used, newBytes, 0, _bufferAvailable);
                        _buffer = newBytes;
                    }
                }

                StartReceive();
            }
            else
            {
                Close();
            }
            
        }

        virtual protected int UnpackProtocolData()
        {
            return _protocolProcess.Unpack(_buffer, _bufferAvailable, OnReceiveData);
        }

        virtual protected byte[] PackProtocolData(byte[] bytes)
        {
            return _protocolProcess.Pack(bytes);
        }

        /// <summary>
        /// 收到数据时触发
        /// </summary>
        /// <param name="protocolData"></param>
        protected void OnReceiveData(byte[] protocolData)
        {
            onReceiveData?.Invoke(this, protocolData);
        }

        /// <summary>
        /// 发送协议数据
        /// </summary>
        /// <param name="bytes"></param>
        virtual public void Send(byte[] bytes)
        {
            if (false == IsConnected)
            {
                return;
            }

            var protocolData = PackProtocolData(bytes);
            SendBytes(protocolData);
        }

        /// <summary>
        /// 直接发送数据
        /// </summary>
        /// <param name="bytes"></param>
        internal void SendBytes(byte[] bytes)
        {
            _sendBufferList.Add(new ArraySegment<byte>(bytes));

            SendBufferList();
        }

        protected void SendBufferList()
        {
            if (false == IsConnected)
            {
                return;
            }

            //如果没有在发送状态，则调用发送
            if (_isSending || _sendBufferList.Count == 0)
            {
                return;
            }

            _isSending = true;
            _sendEA.BufferList = _sendBufferList.ToArray();

            _sendBufferList.Clear();

            bool willRaiseEvent = _socket.SendAsync(_sendEA);
            if (!willRaiseEvent)
            {
                ProcessSend(_sendEA);
            }
        }

        /// <summary>
        /// 处理发送的消息回调（多线程事件）
        /// </summary>
        /// <param name="e"></param>
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                _isSending = false;
                //尝试一次发送
                SendBufferList();
            }
            else
            {
                Close();
            }
        }

        /// <summary>
        /// 关闭客户端连接
        /// </summary>
        /// <param name="isSilently">如果为true，则不会触发任何事件</param>
        public void Close(bool isSilently = false)
        {
            if (null != _socket)
            {
                try
                {
                    _socket.Shutdown(SocketShutdown.Send);
                }
                catch
                {
                }

                _tsa.Clear();
                _socket.Close();
                _socket = null;
                _buffer = null;
                _receiveEA.Dispose();
                _receiveEA = null;
                _sendEA.Dispose();
                _sendEA = null;

                if (false == isSilently)
                {
                    onShutdown?.Invoke(this);
                }
            }
        }
    }
}
