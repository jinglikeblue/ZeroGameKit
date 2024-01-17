using Jing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Zero
{
    /// <summary>
    /// (毫秒)计时器
    /// 可以记录从调用Start开始后经过的时间。中途可以暂停时间流逝。
    /// 时间流逝统计会在独立线程中执行，不会受到其它线程干扰。
    /// </summary>
    public class Chronograph
    {
        enum EState
        {
            /// <summary>
            /// 停止（下次启动时会复位）
            /// </summary>
            STOPPED,

            /// <summary>
            /// 运行中
            /// </summary>
            RUNNING,

            /// <summary>
            /// 时钟暂停了
            /// </summary>
            PAUSED,
        }

        Thread _thread;
        EState _state = EState.STOPPED;

        long _elapsedMilliseconds = 0;

        /// <summary>
        /// 经过的毫秒数
        /// </summary>
        public long ElapsedMilliseconds
        {
            get
            {
                return _elapsedMilliseconds;
            }
        }

        /// <summary>
        /// 是否正在运行
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return _state == EState.RUNNING ? true : false;
            }
        }
        
        /// <summary>
        /// 启动计时器
        /// </summary>
        public void Start()
        {
            if (_state == EState.RUNNING)
            {
                //已经启动的计时器，必须Stop或Pause后才能重新启动
                return;
            }

            if (null != _thread)
            {
                throw new System.Exception("[Chronograph] 计时器错误，需要排查BUG!!!");
            }

            if(_state == EState.STOPPED)
            {
                Reset();
            }

            _state = EState.RUNNING;

            _thread = new Thread(ThreadUpdate);
            _thread.IsBackground = true;
            _thread.Name = $"chronograph_{_thread.ManagedThreadId}";
            _thread.Start();
        }

        /// <summary>
        /// 暂停计时器。保留最后的统计时间。下次启动时会继续统计时间。
        /// </summary>
        public void Pause()
        {
            if (_state == EState.RUNNING)
            {
                _state = EState.PAUSED;
            }
        }

        /// <summary>
        /// 停止计时器。保留最后的统计时间。下次启动时会重置计时器。
        /// </summary>
        public void Stop()
        {
            _state = EState.STOPPED;
        }

        /// <summary>
        /// 重置计时器
        /// </summary>
        void Reset()
        {
            Stop();
            _elapsedMilliseconds = 0;
        }

        void ThreadUpdate()
        {
            /// <summary>
            /// 最后标记的时间
            /// </summary>
            DateTime lastMarkedTime = DateTime.UtcNow;

            while (true)
            {
                Thread.Sleep(1);

                if (_state == EState.STOPPED || _state == EState.PAUSED)
                {
                    break;
                }

                var tn = DateTime.UtcNow - lastMarkedTime;
                lastMarkedTime = DateTime.UtcNow;
                var pastMS = Convert.ToInt64(tn.TotalMilliseconds);
                _elapsedMilliseconds += pastMS;                
            }

            //线程置空表示结束使用
            _thread = null;
        }
    }
}
