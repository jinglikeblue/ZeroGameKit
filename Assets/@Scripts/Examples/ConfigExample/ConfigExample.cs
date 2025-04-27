using Example.Config;
using UnityEngine;
using Zero;
using ZeroGameKit;
using Zero;

namespace Example
{
    public class ConfigExample
    {
        public static void Start()
        {
            var cfg = ConfigMgr.Ins.LoadZeroConfig<TestConfigVO>();
            Debug.Log("配置文件的内容是：");
            Debug.Log(Json.ToJsonIndented(cfg));

            var win = MsgWin.Show("配置文件的内容是", Json.ToJsonIndented(cfg));
            win.SetContentAlignment(TextAnchor.MiddleLeft);
        }

    }
}