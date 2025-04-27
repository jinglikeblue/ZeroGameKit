using System.Collections;
using UnityEngine;
using Zero;
using ZeroGameKit;
using Zero;

namespace Example
{
    public class MsgWinExample
    {
        public static void Start()
        {
            var text = ConfigMgr.Ins.LoadTextConfig("privacy_policy.txt");
            var win = MsgWin.Show("隐私政策", text);
            win.SetContentAlignment(TextAnchor.UpperLeft);
        }
    }
}