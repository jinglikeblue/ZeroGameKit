using UnityEngine;
using ZeroGameKit;

namespace Example
{
    public class AndroidBridgeExample   
    {        
        public static void Start()
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                MsgWin.Show("提示", "当前环境并不是Android实机！");                
                return;
            }
            var bridge = new AndroidJavaClass("pieces.jing.zerolib.UnityBridge");
            bridge.CallStatic<bool>("showToast", "Bridge Test!!!");
        }

    }
}