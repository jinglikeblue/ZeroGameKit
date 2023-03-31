using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace One
{
    /// <summary>
    /// UDP协议发送器
    /// </summary>
    public class UdpSendChannel
    {
        //SocketAsyncEventArgs _sendEA;

        protected Socket _socket;

        public EndPoint RemoteEndPoint { get; private set; }

        /// <summary>
        /// 线程同步器
        /// </summary>
        ThreadSyncActions _tsa;

        /// <summary>
        /// 数据发送队列
        /// </summary>
        //List<ArraySegment<byte>> _sendBufferList = new List<ArraySegment<byte>>();

        /// <summary>
        /// 是否正在发送数据
        /// </summary>
        bool _isSending = false;

        internal UdpSendChannel(Socket socket, string remoteHost, int remotePort, ThreadSyncActions tsa)
        {
            var remoteEndPoint = new IPEndPoint(IPAddress.Parse(remoteHost), remotePort);
            Init(socket, remoteEndPoint, tsa);
        }

        internal UdpSendChannel(Socket socket, EndPoint remoteEndPoint, ThreadSyncActions tsa)
        {
            Init(socket, remoteEndPoint, tsa);
        }

        void Init(Socket socket, EndPoint remoteEndPoint, ThreadSyncActions tsa)
        {            
            _socket = socket;            
            RemoteEndPoint = remoteEndPoint;
            _tsa = tsa;

            //_sendEA = new SocketAsyncEventArgs();
            //_sendEA.Completed += OnAsyncEventCompleted;
            //_sendEA.RemoteEndPoint = RemoteEndPoint;
        }

        /// <summary>
        /// 异步事件完成（多线程事件）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void OnAsyncEventCompleted(object sender, SocketAsyncEventArgs e)
        //{
        //    _tsa.AddToSyncAction(() =>
        //    {
        //        ProcessSend(e);
        //    });
        //}

        public void Dispose()
        {
            //if (_sendEA != null)
            //{
            //    _sendEA.Completed -= OnAsyncEventCompleted;
            //    _sendEA.Dispose();
            //    _sendEA = null;
            //}
            RemoteEndPoint = null;
            //_sendBufferList = null;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="bytes"></param>
        public void Send(byte[] bytes)
        {
            //_sendBufferList.Add(new ArraySegment<byte>(bytes));
            _socket.SendTo(bytes, RemoteEndPoint);
            //SendBufferList();
        }

        public void SendTo(byte[] bytes, IPEndPoint endPoint)
        {
            _socket.SendTo(bytes, endPoint);
        }

        //void SendBufferList()
        //{
        //    //如果没有在发送状态，则调用发送
        //    if (_isSending || _sendBufferList.Count == 0)
        //    {
        //        return;
        //    }

        //    _isSending = true;
        //    _sendEA.BufferList = _sendBufferList.ToArray();            

        //    _sendBufferList.Clear();
            
        //    if (!_socket.SendToAsync(_sendEA))
        //    {
        //        ProcessSend(_sendEA);
        //    }
        //}

        //void ProcessSend(SocketAsyncEventArgs e)
        //{
        //    if (e.SocketError == SocketError.Success)
        //    {
        //        _isSending = false;
        //        //尝试一次发送
        //        SendBufferList();
        //    }
        //    else
        //    {
        //        //Close();
        //    }
        //}
    }
}
