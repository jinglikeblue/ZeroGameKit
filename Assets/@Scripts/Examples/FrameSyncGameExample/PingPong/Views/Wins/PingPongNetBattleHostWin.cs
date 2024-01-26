using One;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using ZeroGameKit;

namespace PingPong
{
    public class PingPongNetBattleHostWin : WithCloseButtonWin
    {
        Text textTitle;
        Text textContent;

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            textTitle.text = $"(IP:{SocketUtility.GetIPv4Address()})等待加入...";
        }
    }
}
