using System;
using System.Collections;
using System.Collections.Generic;
using Zero;

namespace Zero
{
    /// <summary>
    /// 携程队列器，可以将添加的协程方法按照顺序执行
    /// 注意：协程队列是无法循环使用的。
    /// </summary>
    public class CoroutinesQueue
    {
        public enum EState
        {
            /// <summary>
            /// 空闲中
            /// </summary>
            IDLE,

            /// <summary>
            /// 运行中
            /// </summary>
            RUNNING,

            /// <summary>
            /// 已停止
            /// </summary>
            STOPPED,
        }

        /// <summary>
        /// 队列状态
        /// </summary>
        public EState State { get; private set; } = EState.IDLE;

        /// <summary>
        /// 队列长度
        /// </summary>
        public int Count
        {
            get
            {
                return _coroutineList.Count;
            }
        }

        List<IEnumerator> _coroutineList = new List<IEnumerator>();

        /// <summary>
        /// 向队列中添加一个协程方法
        /// </summary>
        /// <param name="coroutine"></param>
        public void Add(IEnumerator coroutine)
        {
            if (State != EState.IDLE)
            {
                throw new Exception($"Can Add When State Is IDLE");
            }
            _coroutineList.Add(coroutine);
        }

        /// <summary>
        /// 清空队列
        /// </summary>
        public void Clear()
        {
            if (State != EState.IDLE)
            {
                throw new Exception($"Can Clear When State Is IDLE");
            }
            
            _coroutineList.Clear();
        }

        /// <summary>
        /// 启动协程队列
        /// </summary>
        public void Play()
        {
            if (State != EState.IDLE)
            {
                throw new Exception($"Can Play When State Is IDLE");
            }

            ILBridge.Ins.StartCoroutine(this, Run());
        }

        IEnumerator Run()
        {
            State = EState.RUNNING;            

            for (var i = 0; i < _coroutineList.Count; i++)
            {
                var coroutine = _coroutineList[i];
                yield return ILBridge.Ins.StartCoroutine(this, coroutine);
            }

            State = EState.STOPPED;
        }

        /// <summary>
        /// 暂停协程队列，下一次调用Play时，会从之前暂停的位置继续执行协程
        /// </summary>
        public void Pause()
        {
            if (State == EState.RUNNING)
            {
                ILBridge.Ins.StopAllCoroutines(this);
                State = EState.IDLE;
            }
        }

        /// <summary>
        /// 停止协程队列，停止后，该协程队列便不可以再使用
        /// </summary>
        public void Stop()
        {
            Pause();
            State = EState.STOPPED;
        }
    }
}
