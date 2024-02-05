using System;
using System.IO;

namespace PingPong
{
    /// <summary>
    /// 网络模块
    /// </summary>
    public class NetModule:IDisposable
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
        /// 最后一次的信息
        /// </summary>
        public NetDelayInfo lastDelayInfo { get; private set; } = null;

        /// <summary>
        /// 平均信息
        /// </summary>
        public NetDelayInfo avgDelayInfo { get; private set; } = null;

        /// <summary>
        /// 最后一次收到PING/PONG协议的时间
        /// </summary>
        public long lastReceivePingPongUTC = 0;

        public NetModule()
        {
            Protocols.Init();
            
            host = new PingPongNetHost();
            client = new PingPongNetClient();
            _netUpdateCommand = new NetUpdateCommand();
            _netUpdateCommand.Excute();
        }

        public void Dispose()
        {
            host?.Stop();
            client?.Stop();
            _netUpdateCommand.Terminate();
        }

        public void RecordDelayInfo(NetDelayInfo info)
        {
            lastDelayInfo = info;

            if (null == avgDelayInfo)
            {
                avgDelayInfo = info;
            }
            else
            {
                avgDelayInfo.c2s = (avgDelayInfo.c2s + info.c2s) >> 1;
                avgDelayInfo.s2c = (avgDelayInfo.s2c + info.s2c) >> 1;
                avgDelayInfo.total = (avgDelayInfo.total + info.total) >> 1;
            }
        }
    }
}