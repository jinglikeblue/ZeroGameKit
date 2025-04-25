using System;
using System.IO;

namespace PingPong
{
    /// <summary>
    /// 网络模块
    /// </summary>
    public class NetModule : IDisposable
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
        private NetUpdateCommand _netUpdateCommand;

        /// <summary>
        /// 心跳
        /// </summary>
        public HeartbeatModel heartbeat { get; private set; }

        public NetModule()
        {
            Protocols.Init();

            host = new PingPongNetHost();
            client = new PingPongNetClient();
            _netUpdateCommand = new NetUpdateCommand();
            _netUpdateCommand.Execute();
            heartbeat = new HeartbeatModel(3000,6000,6000);
        }

        public void Dispose()
        {
            host?.Stop();
            client?.Stop();
            _netUpdateCommand.Terminate();
        }
    }
}