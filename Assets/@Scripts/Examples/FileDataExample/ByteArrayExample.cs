using Jing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroGameKit;

namespace Example
{
    class ByteArrayExample
    {
        public static void Start()
        {

            int i = 1;
            short s = 2;
            long l = 3;
            float f = 2.14f;
            double d = 3.12d;
            string str = "Hello World!";

            ByteArray ba = new ByteArray(4096);
            ba.Write(i);
            ba.Write(s);
            ba.Write(l);
            ba.Write(f);
            ba.Write(d);
            ba.Write(str);

            StringBuilder log = new StringBuilder();

            //有效字节数
            var size = ba.Available;
            ba.SetPos(0);

            log.AppendLine($"有效字节数:{size}");
            log.AppendLine($"----------开始读取数据----------");
            log.AppendLine($"int值:{ba.ReadInt()}");
            log.AppendLine($"short值:{ba.ReadShort()}");
            log.AppendLine($"long值:{ba.ReadLong()}");
            log.AppendLine($"float值:{ba.ReadFloat()}");
            log.AppendLine($"double值:{ba.ReadDouble()}");
            log.AppendLine($"string值:{ba.ReadString()}");


            MsgWin.Show("ByteArray", log.ToString());            
        }
    }
}
