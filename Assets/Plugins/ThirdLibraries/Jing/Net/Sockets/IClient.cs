using System.Runtime.CompilerServices;

namespace Jing.Net
{
    /// <summary>
    /// 客户端接口
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// 是否连接中
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="bufferSize"></param>
        void Connect(string host, int port, int bufferSize);

        /// <summary>
        /// 重连服务器
        /// </summary>
        void Reconnect();

        /// <summary>
        /// 关闭连接
        /// </summary>        
        void Close(bool isSilently = false);

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="bytes"></param>
        void Send(byte[] bytes);

        /// <summary>
        /// 连接成功
        /// </summary>
        event ConnectServerSuccessEvent onConnectSuccess;

        /// <summary>
        /// 连接失败
        /// </summary>
        event ConnectServerFailEvent onConnectFail;

        /// <summary>
        /// 接收到数据
        /// </summary>
        event ReceivedServerDataEvent onReceivedData;

        /// <summary>
        /// 连接断开
        /// </summary>
        event DisconnectedEvent onDisconnected;
    }
}
