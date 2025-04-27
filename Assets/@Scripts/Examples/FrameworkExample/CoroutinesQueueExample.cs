using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using ZeroGameKit;
using Zero;

namespace Example
{
    class CoroutinesQueueExample
    {
        static public void Start()
        {
            UIWinMgr.Ins.Open<CoroutinesQueueExampleWin>();
        }
    }

    class CoroutinesQueueExampleWin : WithCloseButtonWin
    {
        public Button btnStart;
        public Button btnPause;
        public Button btnStop;
        public Button btnClear;

        public Text textLog;

        CoroutinesQueue _queue;

        public void L(string v)
        {
            textLog.text += "\r\n" + v;
        }

        IEnumerator Coroutine(int waitSeconds, string msg)
        {
            Debug.Log($"Start {msg}");
            L($"Start {msg}");
            yield return new WaitForSeconds(waitSeconds);
            Debug.Log($"End {msg}");
            L($"End {msg}");
            yield return new WaitForSeconds(waitSeconds);
        }

        protected override void OnInit(object data)
        {
            base.OnInit(data);

            var queue = new CoroutinesQueue();
            for (var i = 0; i < 10; i++)
            {
                queue.Add(Coroutine(1, i.ToString()));
            }

            _queue = queue;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            btnStart.onClick.AddListener(() =>
            {
                if (_queue.Count > 0)
                {
                    _queue.Play();
                }
                else
                {
                    L("队列长度为空");
                }
                L($"队列状态:{_queue.State}");
            });

            btnPause.onClick.AddListener(() =>
            {
                _queue.Pause();
                L($"队列状态:{_queue.State}");
            });

            btnStop.onClick.AddListener(() =>
            {
                _queue.Stop();
                L($"队列状态:{_queue.State}");
            });

            btnClear.onClick.AddListener(() =>
            {
                _queue.Clear();
                L($"队列状态:{_queue.State}");
            });
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _queue.Stop();
        }
    }
}
