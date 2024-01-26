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
    class TimeUtilityExample
    {
        static public void Start()
        {
            var sb = new StringBuilder();
            
            sb.AppendLine($"当前当地时间：{Log.Zero1(DateTime.Now.ToString())}");
            sb.AppendLine($"当前UTC时间：{Log.Zero1(DateTime.UtcNow.ToString())}");
            sb.AppendLine($"以毫秒为单位当前UTC时间戳：{Log.Zero1(TimeUtility.NowUtcMilliseconds.ToString())}");
            sb.AppendLine($"以秒为单位当前UTC时间戳：{Log.Zero1(TimeUtility.NowUtcSeconds.ToString())}");            

            var msg = MsgWin.Show("TimeUtilityExample", sb.ToString());
            //msg.SetContentAlignment(UnityEngine.TextAnchor.MiddleLeft);
        }
    }
}
