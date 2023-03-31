using Jing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPPair.Pair
{
    class MatchingInfoVO
    {
        public string host;
        public int messagePort;
        public int matchingPort;
        public DateTime refreshTime;

        public byte[] Serialize()
        {
            var ba = new ByteArray();
            ba.Write(host);
            ba.Write(messagePort);
            ba.Write(matchingPort);
            return ba.GetAvailableBytes();
        }

        public void Deserialize(byte[] data)
        {
            var ba = new ByteArray(data);
            host = ba.ReadString();
            messagePort = ba.ReadInt();
            matchingPort = ba.ReadInt();
        }

        public override string ToString()
        {
            return $"[MatchingInfo] host:{host} messagePort:{messagePort} matchingPort:{matchingPort}";
        }
    }
}
