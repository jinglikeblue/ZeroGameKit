using System.Net;

namespace Jing.Net
{
    /// <summary>
    /// 连接通信的通道
    /// </summary>
    public interface IChannel
    {
        /// <summary>
        /// 发送数据给远端
        /// </summary>
        /// <param name="data">二进制数据内容</param>
        void Send(byte[] data);

        /// <summary>
        /// 关闭远端代理
        /// </summary>
        void Close(bool isSilently = false);

        /// <summary>
        /// 是否连接中
        /// </summary>
        bool IsConnected
        {
            get;
        }

        /// <summary>
        /// 收到数据事件
        /// </summary>
        event ReceivedDataEvent onReceivedData;

        /// <summary>
        /// 通道关闭的事件
        /// </summary>
        event ChannelClosedEvent onChannelClosed;

        /// <summary>
        /// 远程终结点
        /// </summary>
        EndPoint RemoteEndPoint { get; }
    }
}
