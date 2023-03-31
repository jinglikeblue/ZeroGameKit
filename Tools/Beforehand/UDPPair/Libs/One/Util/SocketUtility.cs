using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace One
{
    public static class SocketUtility
    {
        /// <summary>
        /// 检查端口是否可用
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool CheckPortUseable(int port)
        {
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();

            #region 检查端口是否被TCP占用
            IPEndPoint[] tcpEndPoints = properties.GetActiveTcpListeners();           
            foreach (IPEndPoint endPoint in tcpEndPoints)
            {
                if (endPoint.Port == port)
                    return true;
            }
            #endregion

            #region 检查端口是否被UDP占用
            IPEndPoint[] udpEndPoints = properties.GetActiveUdpListeners();
            foreach (IPEndPoint endPoint in udpEndPoints)
            {
                if (endPoint.Port == port)
                    return true;
            }
            #endregion

            return false;
        }

        /// <summary>
        /// 获取本机IPv4地址
        /// </summary>
        /// <returns></returns>
        public static IPAddress GetIPv4Address()
        {
            // 获取本机的所有IP地址
            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

            // 输出所有IP地址
            foreach (IPAddress addr in localIPs)
            {
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                {                    
                    return addr;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取本机IPv6地址
        /// </summary>
        /// <returns></returns>
        public static IPAddress GetIPv6Address()
        {
            // 获取本机的所有IP地址
            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

            // 输出所有IP地址
            foreach (IPAddress addr in localIPs)
            {
                if (addr.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    return addr;
                }
            }
            return null;
        }
    }
}
