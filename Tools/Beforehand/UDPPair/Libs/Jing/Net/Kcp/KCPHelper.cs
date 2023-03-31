using KcpProject;
using System;

namespace Jing
{
    /// <summary>
    /// KCP辅助类，对KCP库进行了二次封装，方便Unity业务使用
    /// 独立的数据分片机制，避免KCP库配置问题产生的阻塞会导致数据无法正常收发的情况
    /// TODO:
    /// 1.使用byte[]对象池来进行buff的管理
    /// </summary>
    public class KCPHelper
    {
        #region KCP配置Settings
        /// <summary>
        /// KCP配置
        /// </summary>
        public class Settings
        {
            /// <summary>
            /// 表示会话编号的整数，和tcp的 conv一样，通信双方需保证 conv相同，相互的数据包才能够被认可
            /// </summary>
            public uint conv = 0;

            /// <summary>
            /// 最大传输单元，mtu是链路层设备的固有值，包长度超过mtu的话ip层会将包进行分片，
            /// 如果丢失一个分片，整个包需要重发。所以在kcp中设置的mtu要小于网络中的mtu，kcp中默认1400
            /// </summary>
            public int mtu = 1400;

            /// <summary>
            /// 发送窗口，是一个数值，snd_buf的最大容量
            /// snd_buf: 发送缓存，保存发送中还未收到ack确认的包
            /// </summary>
            public int sndwnd = 32;

            /// <summary>
            /// 接收窗口，是一个数值，rcv_wnd的容量
            /// rcv_wnd: 接收缓存，保存接收到了的包，且此包前面有没接收到的包，如果此包前面的包都接收到了，那么转移到rcv_queue
            /// </summary>
            public int rcvwnd = 32;

            /// <summary>
            /// 是否启动无延迟模式，延迟返回ack包的目的是，在延迟时间内积累更多的ack包一次性发送，节省流量；启动nodelay则响应更快
            /// 0不启用；1启用。
            /// </summary>
            public int nodelay = 0;

            /// <summary>
            /// 内部flush刷新间隔。协议内部工作的 interval，单位毫秒，比如 10ms或者 20ms
            /// </summary>
            public int interval = 100;

            /// <summary>
            /// 快速重传模式，默认0关闭，可以设置2（2次ACK跨越将会直接重传）
            /// </summary>
            public int resend = 0;

            /// <summary>
            /// 是否关闭流控，默认是0代表不关闭，1代表关闭。
            /// </summary>
            public int nc = 0;

            public static Settings GetNormalModeSettings()
            {
                var settings = new Settings();
                settings.nodelay = 0;
                settings.interval = 40;
                settings.resend = 0;
                settings.nc = 0;
                return settings;
            }

            public static Settings GetFastModeSettings()
            {
                var settings = new Settings();
                settings.nodelay = 1;
                settings.interval = 10;
                settings.resend = 2;
                settings.nc = 1;
                return settings;
            }
        }
        #endregion

        /// <summary>
        /// 协议包头大小
        /// </summary>
        const int PACK_HEAD_SIZE = 4;

        /// <summary>
        /// 包数据的缓冲区
        /// </summary>
        ByteArray _packBuffer;

        /// <summary>
        /// KCP协议对象
        /// </summary>
        KCP _kcp;

        /// <summary>
        /// KCP检查时间戳
        /// </summary>
        uint _checkTime = 0;

        /// <summary>
        /// 需要发送的KCP数据
        /// </summary>
        public Action<byte[]> onToSend;

        /// <summary>
        /// 接收到的数据
        /// </summary>
        public Action<byte[]> onReceived;

        /// <summary>
        /// 最大传输单元大小
        /// </summary>
        public uint MSS
        {
            get
            {                
                return _kcp.Mss - PACK_HEAD_SIZE;
            }
        }

        /// <summary>
        /// KCP设置
        /// </summary>
        public Settings settings { get; private set; } = null;

        #region 构造函数
        public KCPHelper(Settings settings = null)
        {
            if (null == settings)
            {
                //默认使用快速模式
                settings = Settings.GetFastModeSettings();
                //最大传输单元设置为500字节，实际
                settings.mtu = 500;
                //设置一个超大的接受发送窗口
                settings.rcvwnd = settings.sndwnd = 512;
            }

            this.settings = settings;

            _kcp = new KCP(settings.conv, OnOutputBytes);

            /* 
             * 该值将会影响数据包归并及分片时候的最大传输单元。
             */
            _kcp.SetMtu(settings.mtu);

            /*
             * 该调用将会设置协议的最大发送窗口和最大接收窗口大小，默认为32. 
             * 这个可以理解为 TCP的 SND_BUF 和 RCV_BUF，只不过单位不一样 
             * SND/RCV_BUF 单位是字节，这个单位是包。
             */
            _kcp.WndSize(settings.sndwnd, settings.rcvwnd);

            /*
             * 工作模式设置
             */
            _kcp.NoDelay(settings.nodelay, settings.interval, settings.resend, settings.nc);

            /*
             * 因为已经自行封装了协议分包机制（KCP自带的发超大数据有问题)，这里直接为false就行。
             */
            _kcp.SetStreamMode(false);
        }
        #endregion

        /// <summary>
        /// 打包数据
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        byte[] PackData(byte[] bytes)
        {
            var totalSize = PACK_HEAD_SIZE + bytes.Length;
            var packed = new ByteArray(totalSize);
            //写入数据的长度
            packed.Write(bytes.Length);
            //写入数据
            packed.Write(bytes);
            return packed.GetAvailableBytes();
        }

        private void OnOutputBytes(byte[] bytes, int length)
        {
            //Debug.Log($"发送KCP数据 size:{length}");

            var tempBytes = new byte[length];
            Array.Copy(bytes, tempBytes, length);

            onToSend?.Invoke(tempBytes);
        }

        public void Update()
        {
            if (0 == _checkTime || _kcp.CurrentMS > _checkTime)
            {
                //Debug.Log($"KCP更新 {DateTime.UtcNow.Millisecond}");
                _kcp.Update();                
                _checkTime = _kcp.Check();
            }
        }

        /// <summary>
        /// 发送的业务数据
        /// </summary>
        /// <param name="bytes"></param>
        public void Send(byte[] bytes)
        {
            if (bytes.Length == 0)
            {
                return;
            }

            //Debug.Log($"发送业务数据大小:{bytes.Length}, 分片数量:{(bytes.Length + MSS - 1) / MSS}");

            bytes = PackData(bytes);
            var toSendLength = bytes.Length;
            int index = 0;
            do
            {
                int length;
                if (toSendLength > _kcp.Mss)
                {
                    length = (int)_kcp.Mss;
                }
                else
                {
                    length = toSendLength;
                }

                int errorCode = _kcp.Send(bytes, index, length);

                switch (errorCode)
                {
                    case 0:
                        //没错
                        break;
                    case -2:
                        throw new Exception($"[{errorCode}] 支持的最大长度为:{255 * _kcp.Mss}, 当前为:{bytes.Length}");
                    default:
                        throw new Exception($"发送协议出错: [{errorCode}]");
                }

                index += length;
                toSendLength -= length;
            }
            while (toSendLength > 0);
        }

        /// <summary>
        /// 接收到的KCP数据，这里处理后通过onReceived派发业务数据
        /// </summary>
        /// <param name="bytes"></param>
        public void KcpInput(byte[] bytes)
        {
            if (bytes.Length == 0)
            {
                return;
            }
            //Debug.Log($"收到KCP数据 size:{bytes.Length}");
            var errorCode = _kcp.Input(bytes, 0, bytes.Length, true, true);
            if (errorCode != 0)
            {
                throw new Exception($"接受KCP数据出错: [{errorCode}]");
            }

            do
            {
                var size = _kcp.PeekSize();

                if (size <= 0)
                {
                    //没有收到数据
                    break;
                }

                if (size < PACK_HEAD_SIZE && null == _packBuffer)
                {
                    //等待一个新的协议，但是收到的数据不够一个包的头
                    break;
                }

                var receivedBytes = new byte[size];
                _kcp.Recv(receivedBytes);

                if (null == _packBuffer)
                {
                    var headBA = new ByteArray(receivedBytes);
                    _packBuffer = new ByteArray(headBA.ReadInt());
                    _packBuffer.Write(headBA.ReadBytes(headBA.ReadableSize));
                }
                else
                {
                    _packBuffer.Write(receivedBytes);
                }

                if (0 == _packBuffer.WriteableSize)
                {
                    //一个数据包收齐了，进行派发
                    DispatchReceivedData(_packBuffer.Bytes);
                    _packBuffer = null;
                }
            }
            while (true);
        }

        void DispatchReceivedData(byte[] data)
        {
            //Debug.Log($"收到业务数据 size:{data.Length}");
            onReceived?.Invoke(data);
        }
    }
}