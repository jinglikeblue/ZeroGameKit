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

        private PingPongNetHost host => Global.Ins.netModule.host;

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            var selfIP = SocketUtility.GetIPv4Address().ToString();
            textTitle.text = $"(IP:{selfIP})等待加入...";
            var part4 = selfIP.Substring(selfIP.LastIndexOf('.') + 1);
            textContent.text = $"你的号码：{part4}";
            host.Start();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            Global.Ins.noticeModule.onHostStart += OnHostStart;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            Global.Ins.noticeModule.onHostStart -= OnHostStart;
        }

        private void OnHostStart()
        {
            UIPanelMgr.Ins.Switch<PingPongGamePanel>();    
            UIWinMgr.Ins.CloseAll();
        }

        protected override void OnBtnCloseClick()
        {
            base.OnBtnCloseClick();
            host.Stop();
        }
    }
}