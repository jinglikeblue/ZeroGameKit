using System.Collections.Generic;
using Jing;

namespace PingPong
{
    /// <summary>
    /// 心跳模型
    /// </summary>
    public class HeartbeatModel
    {
        /// <summary>
        /// 网络延迟记录数量
        /// </summary>
        private const int MAX_NET_DELAY_RECORD_COUNT = 10;

        /// <summary>
        /// 最近发送Ping的时间
        /// </summary>
        public long lastPingSendTime;

        /// <summary>
        /// 最近收到Ping的时间
        /// </summary>
        public long lastPingReceivedTime;

        /// <summary>
        /// 最近发送Pong的时间
        /// </summary>
        public long lastPongSendTime;

        /// <summary>
        /// 最近收到Pong的时间
        /// </summary>
        public long lastPongReceivedTime;

        /// <summary>
        /// Ping发送间隔
        /// </summary>
        public int PingSendIntervalMilliseconds { get; private set; }

        /// <summary>
        /// Ping发送超时阈值
        /// </summary>
        public int PingReceiveTimeoutMilliseconds { get; }

        /// <summary>
        /// Pong接收超时阈值
        /// </summary>
        public int PongReceiveTimeoutMilliseconds { get; private set; }

        /// <summary>
        /// 是否在等待Pong协议。true表示发送了ping还未收到pong
        /// </summary>
        public bool isWaittingPong { get; private set; } = false;

        /// <summary>
        /// 收到的延迟数据链表
        /// </summary>
        private LinkedList<int> _lastNetDelayList;

        /// <summary>
        /// 最近的网络延迟
        /// </summary>
        public int lastNetDelay => _lastNetDelayList.Last.Value;

        /// <summary>
        /// 平均网络延迟
        /// </summary>
        public int avgNetDelay { get; private set; } = 0;

        /// <summary>
        /// 检查是否需要发送Ping协议
        /// </summary>
        /// <returns></returns>
        public bool IsNeedSendPing
        {
            get
            {
                if (TimeUtility.NowUtcMilliseconds - lastPingSendTime >= PingSendIntervalMilliseconds)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// 是否接收Pong超时
        /// </summary>
        public bool IsPongReceivedTimeout
        {
            get
            {
                if (isWaittingPong && TimeUtility.NowUtcMilliseconds - lastPongReceivedTime >= PongReceiveTimeoutMilliseconds)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// 是否接收Ping超时
        /// </summary>
        public bool IsPingReceivedTimeout
        {
            get
            {
                if (TimeUtility.NowUtcMilliseconds - lastPingReceivedTime >= PingReceiveTimeoutMilliseconds)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pingSendIntervalMilliseconds">Ping发送的间隔时间</param>
        /// <param name="pongReceiveTimeoutMilliseconds">相对于最后一次Pong接收，超过了该毫秒值未收到Pong，视为超时。建议为pingSendIntervalMilliseconds的3倍</param>
        /// <param name="pingReceiveTimeoutMilliseconds">相对于最后一次Ping接收，超过了该时间段，视为超时。建议为pingSendIntervalMilliseconds的3倍</param>
        public HeartbeatModel(int pingSendIntervalMilliseconds = 1000, int pongReceiveTimeoutMilliseconds = 3000, int pingReceiveTimeoutMilliseconds = 3000)
        {
            PingSendIntervalMilliseconds = pingSendIntervalMilliseconds;
            PongReceiveTimeoutMilliseconds = pongReceiveTimeoutMilliseconds;
            PingReceiveTimeoutMilliseconds = pingReceiveTimeoutMilliseconds;
            Refresh();
        }

        /// <summary>
        /// 刷新心跳统计模型。一般在连接成功后执行一次。
        /// </summary>
        public void Refresh()
        {
            isWaittingPong = false;
            lastPingSendTime = lastPingReceivedTime = lastPongSendTime = lastPongReceivedTime = TimeUtility.NowUtcMilliseconds;
            _lastNetDelayList = new LinkedList<int>(new int[] { 0 });
            avgNetDelay = 0;
        }

        /// <summary>
        /// Ping发送后调用该方法，更新数据
        /// </summary>
        public void PingSent()
        {
            lastPingSendTime = TimeUtility.NowUtcMilliseconds;
            isWaittingPong = true;
        }

        /// <summary>
        /// 收到Ping协议后调用该方法，更新数据
        /// </summary>
        public void PingReceived()
        {
            lastPingReceivedTime = TimeUtility.NowUtcMilliseconds;
        }

        /// <summary>
        /// Pong发送后调用该方法，更新数据
        /// </summary>
        public void PongSent()
        {
            lastPongSendTime = TimeUtility.NowUtcMilliseconds;
        }

        /// <summary>
        /// 收到Pong协议后调用该方法，更新数据
        /// </summary>
        public void PongReceived()
        {
            lastPongReceivedTime = TimeUtility.NowUtcMilliseconds;
            isWaittingPong = false;

            var netDelay = lastPongReceivedTime - lastPingSendTime;
            _lastNetDelayList.AddLast((int)netDelay);
            if (_lastNetDelayList.Count > MAX_NET_DELAY_RECORD_COUNT)
            {
                _lastNetDelayList.RemoveFirst();
            }

            int temp = 0;
            foreach (var value in _lastNetDelayList)
            {
                temp += value;
            }

            avgNetDelay = temp / _lastNetDelayList.Count;
        }
    }
}