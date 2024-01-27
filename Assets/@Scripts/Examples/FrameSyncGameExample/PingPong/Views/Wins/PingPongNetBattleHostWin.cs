using Jing;
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
