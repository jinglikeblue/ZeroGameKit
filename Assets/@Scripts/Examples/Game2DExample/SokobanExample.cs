using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroGameKit;

namespace Example
{
    class SokobanExample
    {
        static public void Start()
        {
            UIPanelMgr.Ins.Switch<Sokoban.SokobanMenuPanel>();
        }
    }
}
