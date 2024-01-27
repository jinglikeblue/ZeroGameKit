using System;

namespace Jing
{
    /// <summary>
    /// (毫秒)计时器
    /// 可以记录从调用Start开始后经过的时间。中途可以暂停时间流逝。
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
        
        EState _state = EState.STOPPED;

        /// <summary>
        /// 开始时间
        /// </summary>
        DateTime _startTime;

        /// <summary>
        /// 启动时刻的时间戳
        /// </summary>
        public long StartUtcMilliseconds => _startTime.ToUtcMilliseconds();
        

        /// <summary>
        /// 暂停之前经过了的时间
        /// </summary>
        double _elapsedTimeBeforePause = 0;

        /// <summary>
        /// 经过的毫秒数
        /// </summary>
        public long ElapsedMilliseconds
        {
            get
            {
                double elapsedMilliseconds = _elapsedTimeBeforePause;
                if (_state == EState.RUNNING)
                {
                    var delta = DateTime.UtcNow - _startTime;
                    elapsedMilliseconds += delta.TotalMilliseconds;
                }                
                
                return Convert.ToInt64(elapsedMilliseconds);
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

            if(_state == EState.STOPPED)
            {
                Reset();
            }

            _state = EState.RUNNING;
            _startTime = DateTime.UtcNow;
        }

        /// <summary>
        /// 暂停计时器。保留最后的统计时间。下次启动时会继续统计时间。
        /// </summary>
        public void Pause()
        {
            if (_state == EState.RUNNING)
            {
                var delta = DateTime.UtcNow - _startTime;
                _elapsedTimeBeforePause += delta.TotalMilliseconds;

                _state = EState.PAUSED;
            }
        }

        /// <summary>
        /// 停止计时器。保留最后的统计时间。下次启动时会重置计时器。
        /// </summary>
        public void Stop()
        {
            if (_state == EState.RUNNING)
            {
                Pause();
            }
            _state = EState.STOPPED;
        }

        /// <summary>
        /// 重置计时器
        /// </summary>
        void Reset()
        {
            Stop();
            _elapsedTimeBeforePause = 0;
        }
    }
}
