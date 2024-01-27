using System;

namespace Jing
{
    /// <summary>
    /// 时间对象扩展方法
    /// </summary>
    public static class DateTimeExtend
    {        
        /// <summary>
        /// UNIX时间。
        /// 这一刻标志着UNIX操作系统的诞生。UNIX时间是从1970年1月1日00:00:00开始计时的，被称为UNIX纪元或UNIX时间戳。
        /// </summary>
        static readonly DateTime UNIX_EPOCH_TIME = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        /// <summary>
        /// 以毫秒为单位，获取对象的UTC时间戳
        /// 从 1970 年 1 月 1 日 00:00:00 UTC（协调世界时）至今
        /// </summary>
        /// <param name="dataTime"></param>
        /// <returns></returns>
        public static long ToUtcMilliseconds(this DateTime dataTime)
        {
            TimeSpan tn = dataTime - UNIX_EPOCH_TIME;            
            return Convert.ToInt64(tn.TotalMilliseconds);
        }
    }
}
