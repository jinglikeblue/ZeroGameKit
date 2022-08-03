using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroGameKit;

namespace Example
{
    class JsonExample
    {
        public static void Start()
        {
            var vo = new JsonExampleTestVO();
            //数据转换为JSON
            var jsonStr = LitJson.JsonMapper.ToJson(vo);
            //JSON转为数据
            var obj = LitJson.JsonMapper.ToObject<JsonExampleTestVO>(jsonStr);

            var msg = MsgWin.Show("Json", LitJson.JsonMapper.ToPrettyJson(vo));
            msg.SetContentAlignment(UnityEngine.TextAnchor.MiddleLeft);
        }

        class JsonExampleTestVO
        {
            public int[] array;
            //public Dictionary<int, string> map;
            public int i = 1;
            public short s = 2;
            public long l = 3;
            public float f = 2.14f;
            public double d = 3.12d;
            public string str = "Hello World!";

            public JsonExampleTestVO()
            {
                array = new int[] { 9, 5, 2, 7 };
                //map = new Dictionary<int, string>();
                //map[1] = "a";
                //map[2] = "b";
                //map[3] = "c";
                //map[4] = "d";
            }
        }
    }
}
