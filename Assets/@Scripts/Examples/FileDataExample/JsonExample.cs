using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zero;
using ZeroGameKit;

namespace Example
{
    class JsonExample
    {
        public static void Start()
        {
            var vo = new JsonExampleTestVO();
            //数据转换为JSON
            var jsonStr = Json.ToJson(vo);
            //JSON转为数据
            var obj = Json.ToObject<JsonExampleTestVO>(jsonStr);

            var msg = MsgWin.Show("Json", Json.ToJsonIndented(vo));
            msg.SetContentAlignment(UnityEngine.TextAnchor.MiddleLeft);

            #region NewtonSoftJson
            try
            {
                var jsonStr1 = Newtonsoft.Json.JsonConvert.SerializeObject(vo, Newtonsoft.Json.Formatting.Indented);
                Debug.Log($"[NewtonsoftJson] JSON:{jsonStr1}");
                var obj1 = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonStr1, typeof(JsonExampleTestVO));
                var jsonStr2 = Newtonsoft.Json.JsonConvert.SerializeObject(vo, Newtonsoft.Json.Formatting.Indented);
                Debug.Log($"[NewtonsoftJson] JSON_Deserialized:{jsonStr2}");
            }
            catch(Exception e)
            {
                Debug.LogError("[NewtonsoftJson] 测试失败！ILRuntime不支持");
                Debug.LogError(e);
            }
            #endregion
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
