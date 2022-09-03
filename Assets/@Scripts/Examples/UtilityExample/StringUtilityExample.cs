using Jing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zero;
using ZeroGameKit;

namespace Example
{
    class StringUtilityExample
    {
        static public void Start()
        {
            var sb = new StringBuilder();

            string testStr = "HelloWorld\0\0\0\0\0";
            var tucStr = StringUtility.TrimUnusefulChar(testStr);

            sb.AppendLine($"原始字符串 Length：{Log.Zero1(testStr.Length.ToString())}");
            sb.AppendLine($"字符串精简后 Length：{Log.Zero1(tucStr.Length.ToString())}");

            var msg = MsgWin.Show("StringUtilityExample", sb.ToString());            
        }
    }
}
