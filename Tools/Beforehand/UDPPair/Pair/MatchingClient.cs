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

        Dictionary<string, MatchingInfoVO> _matchingMap = new Dictionary<string, MatchingInfoVO>();

        public MatchingInfoVO[] GetMatchingInfoList()
        {
            return _matchingMap.Values.ToArray();
        }

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

            try
            {
                vo.Deserialize(datas);
            }
            catch
            {
                //数据有问题
                vo = null;
            }

            if(null == vo)
            {
                return;
            }

            if(vo.matchingPort == port)
            {
                //是我自己发的消息，过滤
                return;
            }

            List<string> oldKeys = new List<string>();
            //删除过时的数据
            foreach(var kv in _matchingMap)
            {
                if((DateTime.Now - kv.Value.refreshTime).TotalSeconds > 10)
                {
                    oldKeys.Add(kv.Key);
                }
            }

            foreach(var key in oldKeys)
            {
                _matchingMap.Remove(key);
            }

            vo.refreshTime = DateTime.Now;
            _matchingMap[vo.ToString()] = vo;
            
            Console.WriteLine($"[Matching]:{vo.ToString()}");

            Notice.Send(NoticeDefines.PAIR, vo);
        }
    }
}
