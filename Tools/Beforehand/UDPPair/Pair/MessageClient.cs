using Jing;
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


            Notice.AddListener(NoticeDefines.PAIR, OnNotice);
        }

        private void OnNotice(string noticeName, object[] datas)
        {
            var vo = datas[0] as MatchingInfoVO;
            var channel = udp.CreateSendChannel(new IPEndPoint(IPAddress.Parse(vo.host), vo.messagePort));
            channel.Send(Encoding.UTF8.GetBytes($"This Message From {port}"));
        }

        void Loop()
        {
            while (true)
            {
                udp.Refresh();
                Thread.Sleep(10);
            }
        }

        private void OnReceiveData(UdpServer udp, EndPoint remote, byte[] datas)
        {
            var msg = Encoding.UTF8.GetString(datas);
            Console.WriteLine($"[Message]: {msg}");
            //var sendChannel = udp.CreateSendChannel(remote);
            //sendChannel.Send(datas);
        }
    }
}
