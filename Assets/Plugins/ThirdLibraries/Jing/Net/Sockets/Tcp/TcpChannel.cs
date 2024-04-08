using Jing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Jing.Net
{
    public class TcpChannel : IChannel
    {
        /// <summary>
        /// 远程终结点
        /// </summary>
        public EndPoint RemoteEndPoint { get; private set; } = null;

        /// <summary>
        /// 收到数据
        /// </summary>
        public event ReceivedDataEvent onReceivedData;

        /// <summary>
        /// 客户端连接关闭事件
        /// </summary>
        public event ChannelClosedEvent onChannelClosed;

        protected Socket _socket;

        protected byte[] _buffer;

        /// <summary>
        /// 数据发送队列
        /// </summary>
        protected List<ArraySegment<byte>> _sendBufferList = new List<ArraySegment<byte>>();

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
            RemoteEndPoint = socket.RemoteEndPoint;
            CreateProtocolProcess();
            SendLoop();
            ReceiveLoop(bufferSize);
            //ConnectedCheckLoop();
        }

        //async void ConnectedCheckLoop()
        //{
        //    while (IsConnected)
        //    {
        //        await Task.Delay(CONNECTED_CHECK_INTERVAL);
        //    }

        //    Close();
        //}

        /// <summary>
        /// 发送协议的循环
        /// </summary>
        async void SendLoop()
        {
            while (IsConnected)
            {
                ArraySegment<byte>[]? bufferList = null;

                lock (_sendBufferList)
                {
                    if (_sendBufferList.Count > 0)
                    {
                        bufferList = _sendBufferList.ToArray();
                        _sendBufferList.Clear();
                    }
                }

                if (null == bufferList)
                {
                    new ManualResetEvent(false).WaitOne(1);
                    await Task.Yield();
                    continue;
                }

                try
                {
                    await _socket.SendAsync(bufferList, SocketFlags.None);
                }
                catch (Exception ex)
                {
                    Log.E("发送协议失败！");
                    Log.E(ex);
                    Close();
                }
            }
        }

        /// <summary>
        /// 接收协议的循环
        /// </summary>
        async void ReceiveLoop(int bufferSize)
        {
            var buffer = new byte[bufferSize];
            _buffer = buffer;

            while (IsConnected)
            {
                try
                {
                    int bytesRead = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer, _bufferAvailable, _buffer.Length - _bufferAvailable), SocketFlags.None);

                    //bytesRead的值为0时，表示已经达到了文件的结尾（End of File, EOF)
                    if (0 == bytesRead)
                    {
                        Close();
                        break;
                    }

                    _bufferAvailable += bytesRead;
                }
                catch (Exception ex)
                {
                    Log.E(ex);
                }

                //协议处理器处理协议数据
                int used = UnpackProtocolData();

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
            }
        }

        virtual protected void CreateProtocolProcess()
        {
            _protocolProcess = new TcpProtocolProcess();
        }

        virtual protected int UnpackProtocolData()
        {
            return _protocolProcess.Unpack(_buffer, _bufferAvailable, OnReceivedData);
        }

        virtual protected byte[] PackProtocolData(byte[] bytes)
        {
            return _protocolProcess.Pack(bytes);
        }

        /// <summary>
        /// 收到数据时触发
        /// </summary>
        /// <param name="protocolData"></param>
        protected void OnReceivedData(byte[] protocolData)
        {
            onReceivedData?.Invoke(this, protocolData);
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
            lock (_sendBufferList)
            {
                _sendBufferList.Add(new ArraySegment<byte>(bytes));
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
                lock (_socket)
                {
                    try
                    {
                        _socket.Shutdown(SocketShutdown.Receive);
                    }
                    catch
                    {
                    }

                    _socket.Close();
                    _socket = null;
                }
                _buffer = null;

                if (false == isSilently)
                {
                    onChannelClosed?.Invoke(this);
                }
            }
        }
    }
}
