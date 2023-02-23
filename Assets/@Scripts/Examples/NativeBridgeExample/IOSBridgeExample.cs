using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ZeroGameKit;

namespace Example
{
    class IOSBridgeExample
    {
#if UNITY_IPHONE
        [DllImport("__Internal")]
        static extern string _test(string content);
#endif

        public static void Start()
        {
            if (Application.platform != RuntimePlatform.IPhonePlayer )
            {
                MsgWin.Show("提示", "当前环境并不是iOS实机！");
                return;
            }


#if !UNITY_EDITOR && UNITY_IPHONE
            var response = _test("Unity Call C Test");
            MsgWin.Show("response", response);
#endif
        }
    }
}
