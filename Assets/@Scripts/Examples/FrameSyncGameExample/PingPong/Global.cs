using PingPong;
using ZeroHot;

namespace Example
{
    /// <summary>
    /// 全局数据
    /// </summary>
    public class Global : ASingleton<Global>
    {
        /// <summary>
        /// 主机管理类
        /// </summary>
        public PingPongNetHost host { get; private set; }
        
        /// <summary>
        /// 客户机控制类
        /// </summary>
        public PingPongNetClient client { get; private set; }

        protected override void Init()
        {
            host = new PingPongNetHost();
            client = new PingPongNetClient();
            Protocols.Init();
        }

        public override void Destroy()
        {
            host?.Stop();
            client?.Stop();
        }
    }
}