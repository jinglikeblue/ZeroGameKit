using System.Net;

namespace Jing.Net
{
    /// <summary>
    /// 客户端进入的事件
    /// </summary>
    /// <param name="channel"></param>
    public delegate void ClientEnterEvent(IChannel channel);

    /// <summary>
    /// 客户端退出的事件
    /// </summary>
    /// <param name="channel"></param>
    public delegate void ClientExitEvent(IChannel channel);

    /// <summary>
    /// 通道收到数据的事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="data"></param>
    public delegate void ReceivedDataEvent(IChannel channel, byte[] data);

    /// <summary>
    /// 通道关闭的事件
    /// </summary>
    /// <param name="channel"></param>
    public delegate void ChannelClosedEvent(IChannel channel);

    /// <summary>
    /// UdpListener收到数据的事件
    /// </summary>
    /// <param name="remoteEndPoint"></param>
    /// <param name="data"></param>
    internal delegate void UdpListenerReceivedDataEvent(EndPoint remoteEndPoint, byte[] data);

    /// <summary>
    /// UdpServer收到数据的事件
    /// </summary>
    /// <param name="server"></param>
    /// <param name="endPoint"></param>
    /// <param name="data"></param>
    public delegate void UdpServerReceivedDataEvent(UdpServer server, EndPoint endPoint, byte[] data);

    /// <summary>
    /// UdpClient收到数据的事件
    /// </summary>
    /// <param name="client"></param>
    /// <param name="data"></param>
    public delegate void UdpClientReceivedDataEvent(UdpClient client, byte[] data);


    #region Client 委托

    /// <summary>
    /// 连接服务器成功事件
    /// </summary>
    /// <param name="client">客户端</param>
    public delegate void ConnectServerSuccessEvent(IClient client);

    /// <summary>
    /// 连接服务器失败事件
    /// </summary>
    /// <param name="client">客户端</param>
    public delegate void ConnectServerFailEvent(IClient client);

    /// <summary>
    /// 收到服务器数据事件
    /// </summary>
    /// <param name="client">客户端</param>
    /// <param name="data">数据</param>
    public delegate void ReceivedServerDataEvent(IClient client, byte[] data);

    /// <summary>
    /// 和服务器连接断开的事件
    /// </summary>
    /// <param name="client">客户端</param>
    public delegate void DisconnectedEvent(IClient client);

    #endregion
}
