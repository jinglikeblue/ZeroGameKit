using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using ZeroGameKit;

namespace PingPong
{
    public class PingPongNetBattleJoinWin : WithCloseButtonWin
    {
        InputField textInput;
        Button btnConnect;

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
