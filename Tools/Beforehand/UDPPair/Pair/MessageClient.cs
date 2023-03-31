using One;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UDPPair.Pair
{
    class MessageClient
    {
        public int port;

        UdpServer udp;

        public MessageClient()
        {
            port = 8000;
            while (SocketUtility.CheckPortUseable(port))
            {
                port++;
            }

            udp = new UdpServer();
            udp.onReceiveData += OnReceiveData;
            udp.Bind(port, 4096);

            Task.Run(Loop);
        }

        void Loop()
        {
            while (true)
            {
                udp.Refresh();
                Thread.Sleep(1000);
            }
        }

        private void OnReceiveData(UdpServer udp, EndPoint remote, byte[] datas)
        {
            var msg = Encoding.UTF8.GetString(datas);
            Console.WriteLine($"收到的消息:{msg}");
            var sendChannel = udp.CreateSendChannel(remote);
            sendChannel.Send(datas);
        }
    }
}
