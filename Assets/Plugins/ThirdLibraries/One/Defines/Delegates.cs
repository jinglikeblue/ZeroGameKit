using System.Net;

namespace One
{
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

    /// <summary>
    /// WebSocketClient收到数据的事件
    /// </summary>
    /// <param name="client"></param>
    /// <param name="data"></param>
    public delegate void WebSocketClientReceivedDataEvent(WebSocketClient client, byte[] data);

    /// <summary>
    /// KcpClient收到数据的事件
    /// </summary>
    /// <param name="client"></param>
    /// <param name="data"></param>
    public delegate void KcpClientReceivedDataEvent(KcpClient client, byte[] data);

    /// <summary>
    /// KcpServer有Client进入的事件。建立了连接。
    /// </summary>
    /// <param name="channel"></param>
    public delegate void KcpServerClientEnterEvent(IChannel channel);

    /// <summary>
    /// KcpServer有Client退出的事件。断开了连接。
    /// </summary>
    /// <param name="channel"></param>
    public delegate void KcpServerClientExitEvent(IChannel channel);
}
