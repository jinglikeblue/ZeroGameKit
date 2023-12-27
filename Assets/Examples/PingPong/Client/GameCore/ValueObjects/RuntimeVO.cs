using Jing.FixedPointNumber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPong
{
    /// <summary>
    /// 运行时数据
    /// </summary>
    public class RuntimeVO
    {
        /// <summary>
        /// 帧每次更新的时间间隔
        /// </summary>
        public Number frameInterval;

        /// <summary>
        /// 用来运算的帧数据
        /// </summary>
        public FrameData runtimeFrameData;

        /// <summary>
        /// 最新的完成运算的帧数据
        /// </summary>
        public FrameData confirmedFrameData;

        /// <summary>
        /// 帧数据缓存数量限制
        /// </summary>
        public int frameDataCacheLimit;

        /// <summary>
        /// 缓存的帧数据
        /// </summary>
        public LinkedList<FrameData> cachedFrameDatas;
    }
}
