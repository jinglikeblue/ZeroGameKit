using System;

namespace PingPong
{
    /// <summary>
    /// 通知模块
    /// </summary>
    public class NoticeModule
    {
        /// <summary>
        /// 客户端连上的事件
        /// </summary>
        public event Action onClientJoin;

        /// <summary>
        /// 客户端准备好了
        /// </summary>
        public event Action onClientGameReady;

        /// <summary>
        /// 主机通知开始
        /// </summary>
        public event Action onHostStart;
    }
}