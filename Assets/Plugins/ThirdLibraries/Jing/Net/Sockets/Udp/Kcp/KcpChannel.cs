using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Jing.Net
{
    /// <summary>
    /// KCP协议收发通道
    /// </summary>
    public class KcpChannel : IChannel
    {
        public event ReceivedDataEvent onReceivedData;

        public event ChannelClosedEvent onChannelClosed;

        /// <summary>
        /// KCP辅助器
        /// </summary>
        KcpHelper _kcpHelper;

        /// <summary>
        /// UDP协议发送通道
        /// </summary>
        UdpSendChannel _udpSendChannel;

        /// <summary>
        /// 最后一次收到数据的时间
        /// </summary>
        public long LastReceivedDataTime { get; private set; }

        public KcpChannel(UdpSendChannel sendChannel)
        {
            RemoteEndPoint = sendChannel.RemoteEndPoint;
            _kcpHelper = new KcpHelper();
            _kcpHelper.onToSend += OnKcpToSend;
            _kcpHelper.onReceived += OnReceivedKcpData;

            _udpSendChannel = sendChannel;

            LastReceivedDataTime = TimeUtility.NowUtcMilliseconds;

            new Thread(KcpUpdateThread).Start();
        }

        /// <summary>
        /// KCP更新线程
        /// </summary>
        void KcpUpdateThread()
        {
            while (_kcpHelper != null)
            {
                _kcpHelper.Update();
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// 是否连接中
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (_udpSendChannel == null || TimeUtility.NowUtcMilliseconds - LastReceivedDataTime > KcpDefine.KCP_TIMEOUT_LIMIT)
                {
                    return false;
                }
                return true;
            }
        }

        public EndPoint RemoteEndPoint { get; private set; } = null;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Close(bool isSilently = false)
        {
            if (null != _kcpHelper)
            {
                _kcpHelper.onToSend -= OnKcpToSend;
                _kcpHelper.onReceived -= OnReceivedKcpData;
                _kcpHelper = null;
            }

            _udpSendChannel.Dispose();
            _udpSendChannel = null;

            if (!isSilently)
            {
                onChannelClosed?.Invoke(this);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Send(byte[] data)
        {
            _kcpHelper.Send(data);
        }

        /// <summary>
        /// 处理UDP协议收到的数据
        /// </summary>
        internal void ProcessUdpReceivedData(byte[] data)
        {
            _kcpHelper.KcpInput(data);
        }

        /// <summary>
        /// KCP准备了数据需要通过UDP发送的事件
        /// </summary>
        /// <param name="data"></param>
        void OnKcpToSend(byte[] data)
        {
            _udpSendChannel.Send(data);
        }

        /// <summary>
        /// KCP协议解析后收到的数据
        /// </summary>
        /// <param name="data"></param>
        void OnReceivedKcpData(byte[] data)
        {
            LastReceivedDataTime = TimeUtility.NowUtcMilliseconds;
            onReceivedData?.Invoke(this, data);
        }
    }
}
