using Example.FrameSyncGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ZeroGameKit;

namespace Example
{
    class FrameSyncGameExample
    {
        static public void Start()
        {
            PingPong.WorldEntity a = new PingPong.WorldEntity();
            PingPong.WorldEntity b = a;
            a.ball.speed.speed = new Jing.FixedPointNumber.Number(10);
            Debug.Log($"a:{a.ball.speed.speed}");
            Debug.Log($"b:{b.ball.speed.speed}");
            //Debug.Log($"c:{LitJson.JsonMapper.ToJson(a.ball.speed.speed)}");
            //UIWinMgr.Ins.Open<MatchingPanel>();
        }
    }
}
