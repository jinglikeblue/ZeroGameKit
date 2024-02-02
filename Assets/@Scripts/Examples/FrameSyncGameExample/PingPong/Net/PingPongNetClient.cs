using Jing;
using One;
using UnityEngine;

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
        public void Start(string host)
        {
            if (null != _kcpClient)
            {
                return;
            }

            Debug.Log($"[加入HOST] IP:{host}");
            
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
            var body = new Protocols.JoinHostRequest();
            SendProtocol(body);
        }

        public void GameReady()
        {
            var body = new Protocols.GameReadyRequest();
            SendProtocol(body);
        }

        public void Input(byte moveDir)
        {
            var body = new Protocols.InputRequest();
            body.moveDir = moveDir;
            SendProtocol(body);
        }

        public void Ping()
        {
            var body = new Protocols.PingC2S();
            body.clientUTC = TimeUtility.NowUtcMilliseconds;
            SendProtocol(body);
        }

        #endregion

        public void Update()
        {
            _kcpClient.Refresh();
        }
        
        public void SendProtocol(object protocolBody)
        {
            if (null == _kcpClient || null == _kcpClient)
            {
                return;
            }
            
            var data = Protocols.Pack(protocolBody);
            _kcpClient.Send(data);
        }
    }
}
