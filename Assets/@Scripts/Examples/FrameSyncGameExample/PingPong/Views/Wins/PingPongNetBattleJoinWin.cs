using Jing;
using UnityEngine.UI;
using ZeroGameKit;

namespace PingPong
{
    public class PingPongNetBattleJoinWin : WithCloseButtonWin
    {
        Text textTitle;
        InputField textInput;
        Button btnConnect;

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
            
        }
    }
}
