using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zero;
using ZeroHot;
using ZeroGameKit;
using UnityEngine.UI;
using System.Collections;
using UnityEngine;

namespace Example
{
    class CoroutineProxyExample
    {
        static public void Start()
        {
            UIWinMgr.Ins.Open<CoroutineProxyExampleWin>();
        }
    }

    class CoroutineProxyExampleWin : WithCloseButtonWin
    {
        public Button btnStartA;
        public Button btnStartB;
        public Button btnStopA;
        public Button btnStopB;

        public Text textA;
        public Text textB;

        object obj = new object{ };

        Coroutine coroutineA;

        protected override void OnEnable()
        {
            base.OnEnable();

            btnStartA.onClick.AddListener(StartA);
            btnStartB.onClick.AddListener(StartB);
            btnStopA.onClick.AddListener(StopA);
            btnStopB.onClick.AddListener(StopB);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            btnStartA.onClick.RemoveListener(StartA);
            btnStartB.onClick.RemoveListener(StartB);
            btnStopA.onClick.RemoveListener(StopA);
            btnStopB.onClick.RemoveListener(StopB);            
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            //界面销毁的时候，得确保这个Coroutine Proxy中止，否则回调会继续触发。
            ILBridge.Ins.StopAllCoroutines(obj);
        }

        private void StartA()
        {
            if(coroutineA != null)
            {
                StopCoroutine(coroutineA);
            }
            coroutineA = StartCoroutine(CoroutineA());
        }

        private void StartB()
        {
            ILBridge.Ins.StopAllCoroutines(obj);
            //这种方式可以在任意代码位置启动协程，不过请注意协程的销毁。
            //协程会对obj创建一个代理，将IEnumerator和obj关联起来。
            ILBridge.Ins.StartCoroutine(obj, CoroutineB());
        }

        private void StopA()
        {
            StopCoroutine(coroutineA);            
        }

        private void StopB()
        {            
            ILBridge.Ins.StopAllCoroutines(obj);
        }

        IEnumerator CoroutineA()
        {
            int i = 0;
            while (true)
            {
                textA.text = i.ToString();
                i++;
                yield return new WaitForSeconds(0.1f);
            }
        }

        IEnumerator CoroutineB()
        {
            int i = 0;
            while (true)
            {
                textB.text = i.ToString();
                i++;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
