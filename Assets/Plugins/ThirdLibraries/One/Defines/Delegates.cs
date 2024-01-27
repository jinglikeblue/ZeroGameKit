using System.Net;

namespace One
{
    /// <summary>
    /// 接受数据的事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="data"></param>
    public delegate void ReceivedDataEvent(IChannel sender, byte[] data);

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
}
