using Jing.FixedPointNumber;
using System.Collections.Generic;

namespace PingPong
{
    /// <summary>
    /// 游戏核心
    /// </summary>
    public class GameCore
    {
        /// <summary>
        /// 运行时数据
        /// </summary>
        RuntimeModel runtime;

        /// <summary>
        /// 初始化游戏核心
        /// </summary>
        /// <param name="frameIntervalSeconds">帧间隔时间(单位/秒)</param>
        /// <param name="catchedFrameDataCount">缓存的帧数据数量</param>
        public void Init(Number frameInterval, int frameDataCacheLimit = 10)
        {
            runtime = new RuntimeModel(frameInterval, frameDataCacheLimit);
        }

        public void Update(FrameInput frameInput)
        {
            //增加新的一帧
            runtime.IncreaseFrame(frameInput);
            //系统更新
            runtime.SystemUpdate();
            //固化帧数据
            runtime.ConfirmFrameData();
            //缓存帧数据
            runtime.CacheFrameData();          
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
