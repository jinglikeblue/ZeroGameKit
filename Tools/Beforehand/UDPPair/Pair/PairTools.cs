using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UDPPair.Pair
{
    class PairTools
    {
        public int port;        

        public Action onRefreshList;

        public void Start()
        {
            Update();
        }

        IPAddress GetAddress()
        {
            // 获取本机的所有IP地址
            IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

            // 输出所有IP地址
            foreach (IPAddress addr in localIPs)
            {
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                {
                    Console.WriteLine($"IP地址：{addr}");
                    return addr;
                }
            }
            return null;
        }

        bool IsPortInUse(int port)
        {
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] tcpEndPoints = properties.GetActiveTcpListeners();
            IPEndPoint[] udpEndPoints = properties.GetActiveUdpListeners();

            foreach (IPEndPoint endPoint in tcpEndPoints)
            {
                if (endPoint.Port == port)
                    return true;
            }

            foreach (IPEndPoint endPoint in udpEndPoints)
            {
                if (endPoint.Port == port)
                    return true;
            }

            return false;
        }

        public void Update()
        {
            port = 8000;            
            while (IsPortInUse(port))
            {
                port++;
            }

            UdpClient client = new UdpClient(port);

            Task.Run(() => {              
                var localAddress = GetAddress();
                var data = Encoding.UTF8.GetBytes(port.ToString());

                while (true)
                {
                    for(int targetPort = 8000; targetPort < 8010; targetPort++)
                    {
                        //Console.WriteLine($"广播自己的信息  [{IPAddress.Broadcast}:{targetPort}]");
                        client.Send(data, data.Length, IPAddress.Broadcast.ToString(), targetPort);
                    }                   
                    Thread.Sleep(1000);
                }
            });


            Task.Run(() => {

                IPEndPoint remoteEndPoint = null;
                //UdpClient receiveUpdate = new UdpClient(port);
                
                while (true)
                {                    
                    var bytes = client.Receive(ref remoteEndPoint);
                    var targetPort = int.Parse(Encoding.UTF8.GetString(bytes));
                    if (remoteEndPoint.Port != port)
                    {
                        Console.WriteLine($"收到数据: [{remoteEndPoint.Address}:{targetPort}]");
                    }

                    var targetEndPoint = new IPEndPoint(remoteEndPoint.Address, targetPort);                    
                    
                    Thread.Sleep(1000);
                }
            });
        }

        public void Stop()
        {

        }
    }
}
