using Jing;
using One;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UDPPair.Pair
{
    class MatchingClient
    {
        public int port;

        UdpServer udp;

        MatchingInfoVO info;

        public MatchingClient(int messagePort)
        {
            port = 9000;
            while (SocketUtility.CheckPortUseable(port))
            {
                port++;
            }

            info = new MatchingInfoVO();
            info.host = SocketUtility.GetIPv4Address().ToString();
            info.messagePort = messagePort;
            info.matchingPort = port;



            Task.Run(Loop);
        }

        void Loop()
        {
            udp = new UdpServer();
            udp.onReceiveData += OnReceiveData;
            udp.Bind(port, 4096);

            var data = info.Serialize();
            int[] ports = new int[100];
            for(int i = 0; i < ports.Length; i++)
            {
                ports[i] = 9000 + i;
            }

            while (true)
            {
                udp.Broadcast(data, ports);
                udp.Refresh();
                Thread.Sleep(1000);
            }
        }

        private void OnReceiveData(UdpServer udp, EndPoint remote, byte[] datas)
        {
            var vo = new MatchingInfoVO();
            vo.Deserialize(datas);

            if(vo.matchingPort == port)
            {
                //是我自己发的消息，过滤
                return;
            }
            
            Console.WriteLine($"收到的消息:{vo.ToString()}");

            Notice.Send("PAIR", vo);
        }
    }
}
