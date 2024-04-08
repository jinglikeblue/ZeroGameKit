using Jing;
using System.Runtime.CompilerServices;
using System.Threading;
namespace Jing.Net
{
    /// <summary>
    /// 基于UDP实现的KCP客户端
    /// </summary>
    public class KcpClient : IClient
    {
        /// <summary>
        /// 连接成功事件(多线程事件）
        /// </summary>
        public event ConnectServerSuccessEvent onConnectSuccess;

        /// <summary>
        /// 连接断开事件(多线程事件）
        /// </summary>
        public event DisconnectedEvent onDisconnected;

        /// <summary>
        /// 连接失败事件(多线程事件）
        /// </summary>
        public event ConnectServerFailEvent onConnectFail;

        /// <summary>
        /// 收到数据
        /// </summary>
        public event ReceivedServerDataEvent onReceivedData;

        /// <summary>
        /// KCP辅助器
        /// </summary>
        KcpHelper _kcpHelper;

        /// <summary>
        /// UDP客户端
        /// </summary>
        UdpClient _udpClient;

        /// <summary>
        /// 是否连接中
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (_udpClient == null)
                {
                    return false;
                }

                if (TimeUtility.NowUtcMilliseconds - _lastReceivedDataTime > KcpDefine.KCP_TIMEOUT_LIMIT)
                {
                    return false;
                }

                return true;
            }
        }

        long _lastReceivedDataTime = 0;

        /// <summary>
        /// 绑定KCP主机
        /// </summary>
        /// <param name="remoteHost">远程主机地址</param>
        /// <param name="remotePort">远程主机端口</param>
        /// <param name="localPort">本地监听端口</param>
        /// <param name="bufferSize">缓冲区大小</param>
        public void Start(string remoteHost, int remotePort, int localPort, int bufferSize = 4096)
        {
            if (null == _udpClient)
            {
                _kcpHelper = new KcpHelper();
                _kcpHelper.onToSend += OnKcpToSend;
                _kcpHelper.onReceived += OnReceivedKcpData;

                _udpClient = new UdpClient();
                _udpClient.onReceivedData += OnUdpReceivedData;
                _udpClient.Bind(remoteHost, remotePort, localPort, bufferSize);

                _lastReceivedDataTime = TimeUtility.NowUtcMilliseconds;

                new Thread(KcpUpdateThread).Start();
            }
        }

        /// <summary>
        /// KCP更新线程
        /// </summary>
        void KcpUpdateThread()
        {
            while (IsConnected)
            {
                _kcpHelper.Update();
                Thread.Sleep(1);
            }

            Close();
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
            _lastReceivedDataTime = TimeUtility.NowUtcMilliseconds;
            onReceivedData?.Invoke(this, data);
        }

        public void Connect(string host, int port, int bufferSize)
        {
            var localPart = port + 10;
            if (localPart > ushort.MaxValue)
            {
                localPart = port - 10;
            }
            Start(host, port, localPart, bufferSize);
        }

        public void Reconnect()
        {
            //UDP协议不需要重连
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Close(bool isSilently = false)
        {
            if (null != _kcpHelper)
            {
                _kcpHelper.onToSend -= OnKcpToSend;
                _kcpHelper.onReceived -= OnReceivedKcpData;
                _kcpHelper = null;
            }

            if (null != _udpClient)
            {
                _udpClient.onReceivedData -= OnUdpReceivedData;
                _udpClient.Dispose();
                _udpClient = null;

                if (!isSilently)
                {
                    onDisconnected?.Invoke(this);
                }
            }
        }
    }
}
