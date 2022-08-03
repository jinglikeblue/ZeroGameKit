using System.Diagnostics;
using UnityEngine;
using ZeroGameKit;

namespace Example
{
    public class PerformanceExample
    {

        public static void Calculate()
        {
            UIWinMgr.Ins.Open<FibonacciPerformanceWin>();
        }

        public static void CallNative()
        {
            UIWinMgr.Ins.Open<CallNativeCodePerformanceWin>();
        }
    }
}