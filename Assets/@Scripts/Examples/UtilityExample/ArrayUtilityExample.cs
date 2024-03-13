using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zero;
using ZeroGameKit;

namespace Example
{
    class ArrayUtilityExample
    {
        static public void Start()
        {
            var random = new Random();
            List<TestVO> list = new List<TestVO>();
            for(int i = 0; i < 10; i++)
            {
                TestVO vo;
                vo.key = random.Next().ToString();
                vo.value = random.Next();
                list.Add(vo);
            }

            //Zero.ArrayUtility.Sort
            var map = ZeroHot.ArrayUtility.Array2Table<string, TestVO>(list.ToArray(), "key", "test");

            var sb = new StringBuilder();
            sb.AppendLine("-------------数组转换为字典-------------");
            foreach (var pair in map)
            {
                sb.AppendLine($"KEY:{pair.Key}   VALUE:{Json.ToJson(pair.Value)}");
            }


            sb.AppendLine();
            sb.AppendLine("-------------列表排序（按照Value排序）-------------");
            var newList = Jing.ArrayUtility.Sort<TestVO>(list, (a,b) => {
                return ((TestVO)a).value < ((TestVO)b).value;
            });

            for(int i = 0; i < newList.Count; i++)
            {
                sb.AppendLine($"KEY:{newList[i].key}   VALUE:{newList[i].value}");
            }

            var msg = MsgWin.Show("ArrayUtility", sb.ToString());
            //msg.SetContentAlignment(UnityEngine.TextAnchor.MiddleLeft);
        }

        struct TestVO
        {
            public string key;
            public int value;
        }
    }
}
