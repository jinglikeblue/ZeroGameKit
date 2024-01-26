using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using ZeroGameKit;

namespace PingPong
{
    public class PingPongNetBattleMenuWin : WithCloseButtonWin
    {
        Button btnCreateHost;
        Button btnEnterHost;

        protected override void OnEnable()
        {
            base.OnEnable();
            btnCreateHost.onClick.AddListener(OnClickCreateHost);
            btnEnterHost.onClick.AddListener(OnClickEnterHost);
        }

        private void OnClickCreateHost()
        {
            UIWinMgr.Ins.Open<PingPongNetBattleHostWin>();
            Destroy();
        }

        private void OnClickEnterHost()
        {
            UIWinMgr.Ins.Open<PingPongNetBattleJoinWin>();
            Destroy();
        }
    }
}
