using System;

namespace Jing
{
    /// <summary>
    /// 时间工具类
    /// </summary>
    public class TimeUtility
    {
        //static readonly DateTime UNIX_EPOCH_TIME = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        /// <summary>
        /// 以毫秒为单位当前UTC时间
        /// </summary>
        /// <returns></returns>
        public static long NowUtcMilliseconds
        {
            get
            {
                return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                //TimeSpan tn = DateTime.UtcNow - UNIX_EPOCH_TIME;
                //return Convert.ToInt64(tn.TotalMilliseconds);
            }
        }

        /// <summary>
        /// 以秒为单位当前UTC时间
        /// </summary>
        public static long NowUtcSeconds
        {
            get
            {
                return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                //TimeSpan tn = DateTime.UtcNow - UNIX_EPOCH_TIME;
                //return Convert.ToInt64(tn.TotalSeconds);
            }
        }
    }
}
