using Jing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroGameKit;

namespace Example
{
    class DateTimeExtendExample
    {
        static public void Start()
        {
            var dt = new DateTime();
            var ms = dt.ToMillisecondUTC();

            MsgWin.Show("DateTimeExtend", $"UTC时间戳(毫秒): {ms}");
        }
    }
}
