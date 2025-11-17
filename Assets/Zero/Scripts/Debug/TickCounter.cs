using System;
using Jing;

namespace Zero
{
    /// <summary>
    /// 打点计数器。根据Tick调用，可以统计Tick调用的间隔、每秒调用次数等统计数据。
    /// </summary>
    public class TickCounter
    {
        /// <summary>
        /// Tick的最大时间间隔(毫秒)
        /// </summary>
        public long MaxInterval { get; private set; }

        /// <summary>
        /// Tick的最小时间间隔(毫秒)
        /// </summary>
        public long MinInterval { get; private set; }

        /// <summary>
        /// Tick的总次数
        /// </summary>
        public long TickCount { get; private set; }

        /// <summary>
        /// 第一次Tick调用的时间(毫秒)
        /// </summary>
        public long FirstTickTime { get; private set; }

        /// <summary>
        /// 最后一次打点的时间(毫秒)
        /// </summary>
        public long LastTickTime { get; private set; }

        /// <summary>
        /// 最新一次Tick调用距离上一次Tick调用的时间间隔(毫秒)
        /// </summary>
        public long LastTickInterval { get; private set; }

        /// <summary>
        /// 从第一次打点开始到最后一次打点，经过了的时间(毫秒)
        /// </summary>
        public long ElapsedTime => LastTickTime - FirstTickTime;

        /// <summary>
        /// 每秒打点次数
        /// </summary>
        public int TicksPerSecond => (int)((double)TickCount / ElapsedTime * 1000);

        public TickCounter()
        {
            Reset();
        }
        
        /// <summary>
        /// 打点
        /// </summary>
        /// <returns>返回LastTickInterval</returns>
        public long Tick()
        {
            var now = DateTime.UtcNow.ToUtcMilliseconds();
            if (-1 == FirstTickTime)
            {
                //第一次打点
                FirstTickTime = now;
            }
            else
            {
                LastTickInterval = now - LastTickTime;
                MaxInterval = Math.Max(MaxInterval, LastTickInterval);
                MinInterval = Math.Min(MinInterval, LastTickInterval);
            }

            LastTickTime = now;
            TickCount++;
            return LastTickInterval;
        }
        
        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            MaxInterval = long.MinValue;
            MinInterval = long.MaxValue;
            TickCount = 0;
            FirstTickTime = -1;
            LastTickTime = -1;
            LastTickInterval = 0;
        }

        public override string ToString() 
        {
            return $"[lastInterval:{LastTickInterval}, min-max:{MinInterval}-{MaxInterval}, ticksPerSecond:{TicksPerSecond}, tickCount:{TickCount}]";
        }
    }
}