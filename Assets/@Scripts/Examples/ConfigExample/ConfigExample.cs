using Example.Config;
using UnityEngine;
using Zero;
using ZeroGameKit;
using ZeroHot;

namespace Example
{
    public class ConfigExample
    {
        public static void Start()
        {
            var cfg = ConfigMgr.Ins.LoadZeroHotConfig<TestConfigVO>();
            Debug.Log("配置文件的内容是：");
            Debug.Log(LitJson.JsonMapper.ToPrettyJson(cfg));

            var win = MsgWin.Show("配置文件的内容是", LitJson.JsonMapper.ToPrettyJson(cfg));
            win.SetContentAlignment(TextAnchor.MiddleLeft);
        }

    }
}