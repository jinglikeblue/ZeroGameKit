using One;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDPPair.Pair;

namespace UDPPair
{
    class Program
    {


        static void Main(string[] args)
        {            
            new Program();
            Console.ReadKey();
        }

        public Program()
        {
            var msgClient = new MessageClient();
            var matchingClient = new MatchingClient(msgClient.port);
        }
    }
}
