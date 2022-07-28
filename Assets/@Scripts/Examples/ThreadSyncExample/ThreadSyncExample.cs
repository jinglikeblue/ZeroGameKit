using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jing;
using UnityEngine;
using Zero;
using ZeroGameKit;

namespace Example
{
    class ThreadSyncExample
    {
        static public void Start()
        {
            var tse = new ThreadSyncExample();
            tse.RunAsync();
            tse.RunSync();
        }

        int[] data = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        ThreadSyncActions tsa = new ThreadSyncActions();
        StringBuilder log = new StringBuilder();
        MsgWin msg;

        ThreadSyncExample()
        {
            msg = MsgWin.Show("ThreadSyncActions", "代码执行中.....");            
        }

        public void RunAsync()
        {
            log.AppendLine("------------线程各自执行的情况---------------");
            new Thread(ThreadWork).Start();
            new Thread(ThreadWork).Start();
            
        }

        public void RunSync()
        {
            log.AppendLine("------------使用了线程同步器---------------");
            new Thread(ThreadSyncWork).Start();
            new Thread(ThreadSyncWork).Start();

            ILBridge.Ins.StartCoroutine(this, CallTSA());
        }

        IEnumerator CallTSA()
        {            
            yield return new WaitForSeconds(1);
            tsa.RunSyncActions();

            msg.SetContent(log.ToString());
            msg.SetContentAlignment(UnityEngine.TextAnchor.MiddleLeft);
        }

        void ThreadWork()
        {            
            for (int i = 0; i < data.Length; i++)
            {
                //lock (log)
                //{
                    log.AppendLine($"Thread:{Thread.CurrentThread.ManagedThreadId}    Value:{data[i]}");
                //}
            }
        }

        void ThreadSyncWork()
        {
            for (int i = 0; i < data.Length; i++)
            {
                var v = data[i];
                tsa.AddToSyncAction(() =>
                {
                    log.AppendLine($"Thread:{Thread.CurrentThread.ManagedThreadId}    Value:{v}");                    
                });                
            }
        }
    }
}
