using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Jing
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
                    return false;
            }
            #endregion

            #region 检查端口是否被UDP占用
            IPEndPoint[] udpEndPoints = properties.GetActiveUdpListeners();
            foreach (IPEndPoint endPoint in udpEndPoints)
            {
                if (endPoint.Port == port)
                    return false;
            }
            #endregion

            return true;
        }

        /// <summary>
        /// 获取本机IPv4地址
        /// </summary>
        /// <returns></returns>
        public static IPAddress GetIPv4Address()
        {
            #region 方案1
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface iface in interfaces)
            {
                if (iface.OperationalStatus == OperationalStatus.Up)
                {
                    IPInterfaceProperties adapterProperties = iface.GetIPProperties();
                    foreach (UnicastIPAddressInformation ip in adapterProperties.UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            return ip.Address;
                        }
                    }
                }
            }
            #endregion

            #region 如果方案1没有找到，这是方案2
            // 获取本机的主机名
            string hostName = System.Net.Dns.GetHostName();

            // 根据主机名获取本机的IP地址列表
            System.Net.IPAddress[] addresses = System.Net.Dns.GetHostAddresses(hostName);

            foreach (System.Net.IPAddress address in addresses)
            {
                // 判断IP地址是否为IPv4地址以排除IPv6地址
                if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    // 输出IP地址
                    return address;
                }
            }
            #endregion

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
