using PingPong;
using ZeroHot;

namespace Example
{
    /// <summary>
    /// 全局数据
    /// </summary>
    public class Global : ASingleton<Global>
    {
        public PingPongNetHost host;
        public PingPongNetClient client;
        
        protected override void Init()
        {
        }

        public override void Destroy()
        {
            host?.Stop();
            client?.Stop();
        }
    }
}