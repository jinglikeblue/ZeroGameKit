using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroGameKit;
using Zero;
using Zero;
using UnityEngine;

namespace Example
{
    /// <summary>
    /// 调用主工程的性能测试
    /// </summary>
    public class CallNativeCodePerformanceWin : BasePerformanceWin
    {
        protected override void OnInit(object data)
        {
            base.OnInit(data);

            inputFieldCount.characterLimit = 5;
            inputFieldCount.text = "1000";
            
        }

        protected override void StartTest(long count)
        {
            base.StartTest(count);

            while(--count > -1)
            {
                CallNative();
            }

            base.EndTest();
        }

        void CallNative()
        {
            var go = new GameObject();
            go.SetActive(true);
            go.transform.SetParent(transform);
            go.transform.localScale = new Vector3(2, 3, 4);
            go.transform.localPosition = new Vector3(1, 2, 3);
            GameObject.Destroy(go);
        }
    }
}
