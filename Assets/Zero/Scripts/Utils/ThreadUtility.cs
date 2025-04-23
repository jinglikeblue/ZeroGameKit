using System;
using System.Diagnostics;

namespace Zero
{
    /// <summary>
    /// 线程工具类
    /// </summary>
    public static class ThreadUtility
    {
        /// <summary>
        /// 阻塞当前线程，直到满足条件，或者执行超过多少毫秒
        /// </summary>
        /// <param name="conditionFunc">条件方法，返回true时表示结束阻塞</param>
        /// <param name="timeoutMillisecond">阻塞超时毫秒数。阻塞时间超过设定时间时，强制结束阻塞。</param>
        public static void BlockingUntil(Func<bool> conditionFunc, int timeoutMillisecond = int.MaxValue)
        {
            var sw = new Stopwatch();
            sw.Start();
            do
            {
                if (conditionFunc())
                {
                    break;
                }
            } while (sw.ElapsedMilliseconds < timeoutMillisecond);
            sw.Stop();
        }
    }
}