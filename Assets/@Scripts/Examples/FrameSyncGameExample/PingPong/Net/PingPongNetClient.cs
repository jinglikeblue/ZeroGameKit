using One;

namespace PingPong
{
    /// <summary>
    /// 客户机
    /// </summary>
    public class PingPongNetClient
    {
        public const int PORT = 30111;
        
        /// <summary>
        /// KCP客户端
        /// </summary>
        KcpClient _kcpClient;

        /// <summary>
        /// 连接主机
        /// </summary>
        /// <param name="host"></param>
        public void Connect(string host)
        {
            if (null != _kcpClient)
            {
                return;
            }

            _kcpClient = new KcpClient();
            _kcpClient.onReceivedData += KcpClientOnonReceivedData;
            _kcpClient.Start(host, PingPongNetHost.PORT, PORT);
        }

        private void KcpClientOnonReceivedData(KcpClient client, byte[] data)
        {
            Protocols.UnpackAndDispatch(data);
        }

        public void Stop()
        {
            if (null == _kcpClient)
            {
                return;
            }

            _kcpClient.onReceivedData -= KcpClientOnonReceivedData;
            _kcpClient.Dispose();
            _kcpClient = null;
        }

        #region 业务协议

        public void JoinHost()
        {
            
        }

        public void GameReady()
        {
            
        }

        public void Input(byte moveDir)
        {
            
        }

        public void Ping()
        {
            
        }

        #endregion
    }
}
