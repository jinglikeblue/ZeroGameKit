using System.Net;

namespace One
{
    /// <summary>
    /// 接受数据的事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="data"></param>
    public delegate void ReceiveDataEvent(IChannel sender, byte[] data);

    /// <summary>
    /// UdpListener收到数据的事件
    /// </summary>
    /// <param name="remoteEndPoint"></param>
    /// <param name="data"></param>
    internal delegate void UdpListenerReceiveDataEvent(EndPoint remoteEndPoint, byte[] data);
}
