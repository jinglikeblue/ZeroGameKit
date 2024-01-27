using Jing;
using One;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PingPong
{
    /// <summary>
    /// 主机
    /// </summary>
    public class PingPongNetHost
    {
        public const int PORT = 30101;

        /// <summary>
        /// UDP服务
        /// </summary>
        UdpServer _server;

        /// <summary>
        /// KCP
        /// </summary>
        KCPHelper _kcp;

        /// <summary>
        /// 启动服务
        /// </summary>
        public void Start()
        {
            if (null == _server)
            {
                Debug.Log($"[创建HOST] IP:{SocketUtility.GetIPv4Address()}");
                _server = new UdpServer();                
                _server.onReceivedData += OnReceivedData;
                _server.Bind(PORT, 4096);

                _kcp = new KCPHelper();
                _kcp.onToSend += OnToSend;
                _kcp.onReceived += OnReceived;
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            if (_server != null)
            {
                Debug.Log($"[停止HOST]");
                _server.Dispose();
                _server = null;
            }
        }

        public void Update()
        {
            _server.Refresh();
        }

        /// <summary>
        /// 收到了UDP数据
        /// </summary>
        /// <param name="server"></param>
        /// <param name="ep"></param>
        /// <param name="data"></param>
        void OnReceivedData(UdpServer server, EndPoint ep, byte[] data)
        {
            _kcp.KcpInput(data);
        }

        public void Send(byte[] data)
        {
            _kcp.Send(data);
        }

        private void OnToSend(byte[] data)
        {            
            
        }

        private void OnReceived(byte[] data)
        {
            
        }
    }
}
