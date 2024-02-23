using Jing;
namespace One
{
    /// <summary>
    /// 基于UDP实现的KCP客户端
    /// </summary>
    public class KcpClient
    {
        /// <summary>
        /// 收到协议数据的事件
        /// </summary>
        public event KcpClientReceivedDataEvent onReceivedData;

        /// <summary>
        /// KCP辅助器
        /// </summary>
        KCPHelper _kcpHelper;

        /// <summary>
        /// UDP客户端
        /// </summary>
        UdpClient _udpClient;

        /// <summary>
        /// 绑定KCP主机
        /// </summary>
        /// <param name="remoteHost">远程主机地址</param>
        /// <param name="remotePort">远程主机端口</param>
        /// <param name="localPort">本地监听端口</param>
        /// <param name="bufferSize">缓冲区大小</param>
        public void Start(string remoteHost, int remotePort, int localPort, ushort bufferSize = 4096)
        {
            if (null == _udpClient)
            {
                _kcpHelper = new KCPHelper();
                _kcpHelper.onToSend += OnKcpToSend;
                _kcpHelper.onReceived += OnReceivedKcpData;

                _udpClient = new UdpClient();                
                _udpClient.onReceivedData += OnUdpReceivedData;
                _udpClient.Bind(remoteHost, remotePort, localPort, bufferSize);                
            }
        }

        /// <summary>
        /// 处理UDP协议收到的数据
        /// </summary>
        private void OnUdpReceivedData(UdpClient client, byte[] data)
        {
            _kcpHelper?.KcpInput(data);
        }

        /// <summary>
        /// 发送业务数据
        /// </summary>
        /// <param name="bytes"></param>
        public void Send(byte[] bytes)
        {
            _kcpHelper?.Send(bytes);
        }

        /// <summary>
        /// KCP准备了数据需要通过UDP发送的事件
        /// </summary>
        /// <param name="data"></param>
        void OnKcpToSend(byte[] data)
        {
            _udpClient.Send(data);
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
        /// 刷新通信
        /// </summary>
        public void Refresh()
        {
            _udpClient?.Refresh();
            _kcpHelper?.Update();
        }

        public void Dispose()
        {
            if (null != _udpClient)
            {
                _udpClient.onReceivedData -= OnUdpReceivedData;
                _udpClient.Dispose();
                _udpClient = null;
            }            

            if (null != _kcpHelper)
            {
                _kcpHelper.onToSend -= OnKcpToSend;
                _kcpHelper.onReceived -= OnReceivedKcpData;
                _kcpHelper = null;
            }
        }
    }
}
