using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace One
{
    /// <summary>
    /// 基于WebSocket的连接通道
    /// </summary>
    public class WebSocketChannel : TcpChannel
    {
        /// <summary>
        /// 通道是否升级完成
        /// </summary>
        public bool IsUpgrade { get; private set; } = false;

        WebSocketProtocolProcess _protocolProcess;

        bool _isUpgradeRequester = false;

        /// <summary>
        /// WebSocket升级结果
        /// </summary>
        internal event Action<WebSocketChannel, bool> onUpgradeResult;

        public WebSocketChannel(Socket socket, int bufferSize) : base(socket, bufferSize)
        {
        }

        protected override void CreateProtocolProcess()
        {
            _protocolProcess = new WebSocketProtocolProcess();
        }

        protected override int UnpackProtocolData()
        {
            if(false == IsUpgrade)
            {
                if(_isUpgradeRequester)
                {
                    //处理升级协议返回数据
                    return UpgradeResponse(_buffer, _bufferAvailable);
                }
                else
                {
                    //处理升级协议请求数据
                    return Upgrade(_buffer, _bufferAvailable);
                }                
            }

            return _protocolProcess.Unpack(_buffer, _bufferAvailable, OnReceiveData);
        }

        private void OnReceiveData(WebSocketProtocolProcess.EOpcode op, byte[] data)
        {
            switch (op)
            {
                case WebSocketProtocolProcess.EOpcode.CONTINUE:
                    break;
                case WebSocketProtocolProcess.EOpcode.TEXT:
                case WebSocketProtocolProcess.EOpcode.BYTE:
                    OnReceiveData(data);
                    break;
                case WebSocketProtocolProcess.EOpcode.CLOSE:
                    Close();
                    break;
                case WebSocketProtocolProcess.EOpcode.PING:
                    SendBytes(_protocolProcess.CreatePongFrame());
                    break;
                case WebSocketProtocolProcess.EOpcode.PONG:
                    break;
            }
        }

        protected override byte[] PackProtocolData(byte[] bytes)
        {
            if(false == IsUpgrade)
            {
                throw new Exception("WebSocket协议未升级，无法发送数据");
            }

            return _protocolProcess.Pack(bytes);
        }

        /// <summary>
        /// 请求升级WebSocket协议
        /// </summary>
        public void RequestUpgrade()
        {
            _isUpgradeRequester = true;

            byte[] responseBytes = _protocolProcess.CreateUpgradeRequest();
            //请求升级
            SendBytes(responseBytes);
        }

        /// <summary>
        /// 升级协议为WebSocket协议
        /// </summary>
        int Upgrade(byte[] buffer, int bufferAvailable)
        {
            //获取客户端发来的升级协议KEY
            ByteArray ba = new ByteArray(buffer, bufferAvailable);
            string clientRequest = ba.ReadStringBytes(Encoding.ASCII, ba.ReadEnableSize);
            string[] datas = clientRequest.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            string value = null;
            try
            {
                for (int i = 0; i < datas.Length; i++)
                {
                    if (datas[i].Contains("Sec-WebSocket-Key"))
                    {
                        string[] keyValue = datas[i].Split(':');
                        value = keyValue[1].Trim();
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.E(e.Message);
                value = null;
            }

            if (null == value)
            {
                Log.E("WebSocket协议升级失败");
                onUpgradeResult?.Invoke(this, false);
                Close();
                return 0;
            }

            byte[] responseBytes = _protocolProcess.CreateUpgradeResponse(value);

            //回执升级协议
            SendBytes(responseBytes);
            
            IsUpgrade = true;
            onUpgradeResult?.Invoke(this, true);
            return bufferAvailable;
        }

        ///// <summary>
        ///// 升级协议的回复处理
        ///// </summary>
        int UpgradeResponse(byte[] buffer, int bufferAvailable)
        {
            //获取服务器发来的升级确认
            ByteArray ba = new ByteArray(buffer, bufferAvailable);
            string clientRequest = ba.ReadStringBytes(Encoding.ASCII, ba.ReadEnableSize);
            string[] datas = clientRequest.Split(new String[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                for (int i = 0; i < datas.Length; i++)
                {
                    if (datas[i].Contains("Sec-WebSocket-Accept"))
                    {
                        IsUpgrade = true;
                        onUpgradeResult?.Invoke(this, true);
                        Log.I("WS协议升级成功！");
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.E(e.Message);
            }

            if(false == IsUpgrade)
            {
                Log.E("WebSocket协议升级失败");
                onUpgradeResult?.Invoke(this, false);
                Close();
            }

            return bufferAvailable;
        }
    }
}
