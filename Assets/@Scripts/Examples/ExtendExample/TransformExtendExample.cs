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
    class TransformExtendExample
    {
        static public void Start()
        {
            Transform t;
            var mw = MsgWin.Show("TransformExtend", "通过扩展改变坐标");
            t = mw.transform;
            mw.onShow += () =>
            {
                t.SetLocalX(0);
                t.SetLocalY(100);
                t.SetLocalZ(-1);
            };
        }
        
    }
}
