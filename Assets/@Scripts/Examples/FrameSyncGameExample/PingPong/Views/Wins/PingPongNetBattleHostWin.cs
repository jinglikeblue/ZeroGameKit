using Example;
using Jing;
using UnityEngine.UI;
using ZeroGameKit;

namespace PingPong
{
    public class PingPongNetBattleHostWin : WithCloseButtonWin
    {
        Text textTitle;
        Text textContent;

        private PingPongNetHost host => Global.Ins.host;

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            var selfIP = SocketUtility.GetIPv4Address().ToString();
            textTitle.text = $"(IP:{selfIP})等待加入...";
            var part4 = selfIP.Substring(selfIP.LastIndexOf('.') + 1);
            textContent.text = $"你的号码：{part4}";
            host.Start();
        }

        protected override void OnBtnCloseClick()
        {
            base.OnBtnCloseClick();
            host.Stop();
        }
    }
}