using Jing;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace One
{
    /// <summary>
    /// 基于UDP实现的KCP服务端
    /// </summary>
    public class KcpServer
    {
        /// <summary>
        /// 新的客户端进入的事件
        /// </summary>
        public event KcpServerClientEnterEvent onClientEnter;

        /// <summary>
        /// 客户端退出的事件
        /// </summary>
        public event KcpServerClientExitEvent onClientExit;

        /// <summary>
        /// UDP服务
        /// </summary>
        UdpServer _udpServer;

        /// <summary>
        /// KCP通信通道表
        /// </summary>        
        Dictionary<EndPoint, KcpChannel> _channelDic;

        /// <summary>
        /// 启动KCP服务
        /// </summary>
        /// <param name="port">服务监听的端口</param>
        /// <param name="bufferSize">套接字缓冲区大小</param>
        public void Start(int port, ushort bufferSize = 4096)
        {
            if (null == _udpServer)
            {
                _channelDic = new Dictionary<EndPoint, KcpChannel>();
                _udpServer = new UdpServer();
                _udpServer.onReceivedData += OnReceivedUdpData;
                _udpServer.Bind(port, bufferSize);
            }
        }

        /// <summary>
        /// 关闭KCP服务
        /// </summary>
        public void Close()
        {
            if (null != _udpServer)
            {                
                _udpServer.Dispose();
                _udpServer = null;
                foreach(var channel in _channelDic.Values)
                {
                    channel.Close();
                }
                _channelDic = null;
            }
        }

        /// <summary>
        /// 该方法用来刷新Kcp服务
        /// </summary>
        public void Refresh()
        {
            //刷新UDP服务
            _udpServer.Refresh();

            //刷新各个KCP通道
            var channelList = _channelDic.Values.ToList();
            var count = channelList.Count;
            while(--count > -1)
            {
                channelList[count].Refresh();
            }            
        }

        /// <summary>
        /// UDP收到了数据，需要传递给KCP处理的事件
        /// </summary>
        /// <param name="server"></param>
        /// <param name="endPoint"></param>
        /// <param name="data"></param>
        private void OnReceivedUdpData(UdpServer server, EndPoint endPoint, byte[] data)
        {
            KcpChannel kcpChannel;
            if (false == _channelDic.ContainsKey(endPoint))
            {
                var udpSendChannel = _udpServer.CreateSendChannel(endPoint);
                kcpChannel = new KcpChannel(udpSendChannel);
                _channelDic.Add(endPoint, kcpChannel);
                onClientEnter?.Invoke(kcpChannel);
            }
            else
            {
                kcpChannel = _channelDic[endPoint];
            }

            kcpChannel.ProcessUdpReceivedData(data);            
        }
    }
}
