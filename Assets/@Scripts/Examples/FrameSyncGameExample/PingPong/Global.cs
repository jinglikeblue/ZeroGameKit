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

        /// <summary>
        /// 网络更新器
        /// </summary>
        public NetUpdateCommand _netUpdateCommand { get; private set; }

        protected override void Init()
        {
            host = new PingPongNetHost();
            client = new PingPongNetClient();
            Protocols.Init();
            _netUpdateCommand = new NetUpdateCommand();
            _netUpdateCommand.Excute();
        }

        public override void Destroy()
        {
            host?.Stop();
            client?.Stop();
            _netUpdateCommand.Terminate();
        }
    }
}