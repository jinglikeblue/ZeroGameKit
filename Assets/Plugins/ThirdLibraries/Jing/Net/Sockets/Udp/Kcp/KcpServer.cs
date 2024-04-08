using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Jing.Net
{
    /// <summary>
    /// 基于UDP实现的KCP服务端
    /// </summary>
    public class KcpServer : IServer
    {
        /// <summary>
        /// 新的客户端进入的事件
        /// </summary>
        public event ClientEnterEvent onClientEnter;

        /// <summary>
        /// 客户端退出的事件
        /// </summary>
        public event ClientExitEvent onClientExit;

        /// <summary>
        /// UDP服务
        /// </summary>
        UdpServer? _udpServer;

        /// <summary>
        /// KCP通信通道表
        /// </summary>        
        ConcurrentDictionary<EndPoint, KcpChannel>? _channelDic;

        public bool IsAlive => _channelDic == null ? false : true;

        /// <summary>
        /// 启动KCP服务
        /// </summary>
        /// <param name="port">服务监听的端口</param>
        /// <param name="bufferSize">套接字缓冲区大小</param>
        public void Start(int port, int bufferSize = 4096)
        {
            if (null == _udpServer)
            {
                _channelDic = new ConcurrentDictionary<EndPoint, KcpChannel>();
                _udpServer = new UdpServer();
                _udpServer.onReceivedData += OnReceivedUdpData;
                _udpServer.Bind(port, bufferSize);
                CleanAllDeadChannelLoop();
            }
        }

        /// <summary>
        /// 关闭KCP服务
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Close()
        {
            if (null != _udpServer)
            {
                _udpServer.Dispose();
                _udpServer = null;
                foreach (var channel in _channelDic.Values)
                {
                    channel.Close();
                }

                _channelDic = null;
            }
        }

        /// <summary>
        /// 清理断线的通道
        /// </summary>
        async void CleanAllDeadChannelLoop()
        {
            while (null != _udpServer)
            {
                var endPoints = _channelDic.Keys.ToArray();
                for (int i = 0; i < endPoints.Length; i++)
                {
                    var ep = endPoints[i];
                    if (false == _channelDic[ep].IsConnected)
                    {
                        //链接断开了，清理掉
                        if (_channelDic.TryRemove(ep, out var channel))
                        {
                            channel.Close();
                        }
                    }
                }

                await Task.Delay(10000);
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
            if (false == _channelDic.ContainsKey(endPoint) || !_channelDic[endPoint].IsConnected)
            {
                var udpSendChannel = _udpServer.CreateSendChannel(endPoint);
                kcpChannel = new KcpChannel(udpSendChannel);
                _channelDic[endPoint] = kcpChannel;
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