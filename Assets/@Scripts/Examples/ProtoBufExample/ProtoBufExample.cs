using Example.ProtoBuf;
using Google.Protobuf;
using ZeroGameKit;

namespace Example
{
    public class ProtoBufExample
    {
        public static void Start()
        {
            var pbe = new ProtoBufExample();
            pbe.Run();
        }

        public void Run()
        {
            Example.ProtoBuf.data data = new Example.ProtoBuf.data();
            data.IntValue = 1;
            data.LongValue = 2;
            data.StrValue = "fuck you";
            data.IntArray.Add(1);
            data.IntArray.Add(2);
            data.IntArray.Add(3);
            data.IntArray.Add(4);
            data.IntArray.Add(5);

            var re = new Example.ProtoBuf.rsp_enter();
            re.RoomId = 99;
            re.MapId = 77;
            data.ObjectList.Add(re);
            re.MapId = 76;
            data.ObjectList.Add(re);
            data.ObjectList.Add(re);
            data.ObjectList.Add(re);


            var mem = new System.IO.MemoryStream();
            data.WriteTo(mem);

            mem.Position = 0;
            var bytes = mem.ToArray();


            //File.WriteAllBytes("test.bytes", bytes);

            var chat1 = Example.ProtoBuf.data.Parser.ParseFrom(bytes);

            MsgWin.Show("´úÂëÊä³ö", ProtoBufDumper.DumpAsString(chat1));
        }
    }
}