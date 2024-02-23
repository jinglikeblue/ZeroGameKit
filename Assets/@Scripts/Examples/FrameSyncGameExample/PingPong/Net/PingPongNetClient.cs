using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using Example;
using Jing;
using One;
using Debug = UnityEngine.Debug;

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
        /// 是否活跃中
        /// </summary>
        public bool IsActive => _kcpClient == null ? false : true;

        /// <summary>
        /// 网络关闭事件
        /// </summary>
        public event Action onClose;

        /// <summary>
        /// 心跳
        /// </summary>
        public HeartbeatModel heartbeat => Global.Ins.netModule.heartbeat;

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
            
            heartbeat.Refresh();
        }

        private void KcpClientOnonReceivedData(KcpClient client, byte[] data)
        {
            var md5 = MD5Helper.GetShortMD5(new MemoryStream(data), true);
            Debug.Log($"收到协议 [size:{data.Length}] [md5:{md5}]");
            Protocols.UnpackAndDispatch(data);
        }

        /// <summary>
        /// 停止网络
        /// </summary>
        /// <param name="isSliently"></param>
        public void Stop(bool isSliently = true)
        {
            if (null == _kcpClient)
            {
                return;
            }
            Debug.Log($"停止CLIENT");
            _kcpClient.onReceivedData -= KcpClientOnonReceivedData;
            _kcpClient.Dispose();
            _kcpClient = null;

            if (!isSliently)
            {
                onClose?.Invoke();
            }
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
            
            heartbeat.PingSent();
        }

        #endregion

        public void Update()
        {
            _kcpClient?.Refresh();

            NetworkCheck();
        }
        
        /// <summary>
        /// 网络检查
        /// </summary>
        private void NetworkCheck()
        {
            if (false == IsActive)
            {
                return;
            }

            if (heartbeat.IsPongReceivedTimeout)
            {
                Stop(false);
                return;
            }

            if (heartbeat.IsNeedSendPing)
            {
                Ping();
            }
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
