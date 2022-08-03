using UnityEngine;
using Zero;
using ZeroHot;

namespace Example
{
    /// <summary>
    /// 单例类使用示例
    /// </summary>
    public class SingletonClassExample
    {
        class SingletonDemo : ZeroHot.ASingleton<SingletonDemo>
        {
            public string value = "I am a singleton class: value";

            public override void Destroy()
            {
                Debug.Log(Log.Blue("singleton class Destroy() run"));
            }

            protected override void Init()
            {
                Debug.Log(Log.Blue("singleton class Init() run"));
            }
        }

        public static void Start()
        {
            Debug.Log(Log.Blue($"访问单例类：{SingletonDemo.Ins.value}"));
            Debug.Log(Log.Blue("销毁单例类"));
            SingletonDemo.ResetIns();            
        }        
    }
}