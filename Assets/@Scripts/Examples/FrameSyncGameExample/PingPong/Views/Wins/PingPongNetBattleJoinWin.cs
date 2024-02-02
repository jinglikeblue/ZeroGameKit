using Example;
using Jing;
using UnityEngine;
using UnityEngine.UI;
using ZeroGameKit;

namespace PingPong
{
    public class PingPongNetBattleJoinWin : WithCloseButtonWin
    {
        Text textTitle;
        InputField textInput;
        Button btnConnect;

        private PingPongNetClient client => Global.Ins.client;

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            textTitle.text = $"(IP:{SocketUtility.GetIPv4Address()})加入对局";
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            btnConnect.onClick.AddListener(OnClickConnect);
        }

        private void OnClickConnect()
        {
            var selfIP = SocketUtility.GetIPv4Address().ToString();
            var part1 = selfIP.Substring(0, selfIP.LastIndexOf('.') + 1);
            var host = $"{part1}{textInput.text.Trim()}";
            client.Start(host);
            client.JoinHost();
        }
    }
}
