using Example.FrameSyncGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroGameKit;

namespace Example
{
    class FrameSyncGameExample
    {
        static public void Start()
        {
            UIWinMgr.Ins.Open<MatchingPanel>();
        }
    }
}
