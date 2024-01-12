using Example.FrameSyncGame;
using Jing.FixedPointNumber;
using PingPong;
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
    class FrameSyncGameExample
    {
        static public void Start()
        {            
            UIPanelMgr.Ins.Switch<PingPongMenuPanel>();
        }
    }
}
