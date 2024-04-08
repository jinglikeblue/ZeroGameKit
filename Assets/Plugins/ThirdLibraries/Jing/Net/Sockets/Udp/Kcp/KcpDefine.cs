using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jing.Net
{
    /// <summary>
    /// KCP协议相关定义
    /// </summary>
    public static class KcpDefine
    {
        /// <summary>
        /// KCP超时限制，如果超过这个时间没有收到数据，则视为断线
        /// </summary>
        public const long KCP_TIMEOUT_LIMIT = 1000 * 60;
    }
}
