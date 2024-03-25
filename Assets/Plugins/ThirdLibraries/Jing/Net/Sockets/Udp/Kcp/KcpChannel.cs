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
        KCPHelper _kcpHelper;

        /// <summary>
        /// UDP协议发送通道
        /// </summary>
        UdpSendChannel _udpSendChannel;

        public KcpChannel(UdpSendChannel sendChannel)
        {
            _kcpHelper = new KCPHelper();
            _kcpHelper.onToSend += OnKcpToSend;
            _kcpHelper.onReceived += OnReceivedKcpData;

            _udpSendChannel = sendChannel;
        }

        /// <summary>
        /// 是否连接上了
        /// </summary>
        public bool IsConnected
        {
            get
            {
                //TODO 还没想好
                return true;
            }
        }

        public void Close(bool isSilently = false)
        {
            //TODO 还没想好
        }

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
            onReceivedData?.Invoke(this, data);
        }

        /// <summary>
        /// 刷新通道的数据处理
        /// </summary>
        public void Refresh()
        {
            _kcpHelper.Update();
        }
    }
}
