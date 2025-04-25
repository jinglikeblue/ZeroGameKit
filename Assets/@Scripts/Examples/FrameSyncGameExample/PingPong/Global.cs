using Jing;
using ZeroHot;

namespace PingPong
{
    /// <summary>
    /// 全局数据
    /// </summary>
    public class Global : BaseSingleton<Global>
    {
        /// <summary>
        /// 网络模块
        /// </summary>
        public NetModule netModule { get; private set; }
        
        /// <summary>
        /// 通知模块
        /// </summary>
        public NoticeModule noticeModule { get; private set; }

        protected override void Init()
        {
            netModule = new NetModule();
            noticeModule = new NoticeModule();
        }

        public override void Destroy()
        {
            netModule.Dispose();
        }
    }
}