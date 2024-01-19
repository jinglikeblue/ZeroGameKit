using Jing.FixedPointNumber;
using System.Collections.Generic;
using Zero;

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
        /// 刷新间隔
        /// </summary>
        public int FrameInterval
        {
            get
            {
                var ms = runtime.vo.frameInterval * 1000;
                return ms.ToInt();
            }
        }

        public FrameData FrameData
        {
            get
            {
                return runtime.vo.confirmedFrameData;
            }
        }

        /// <summary>
        /// 初始化游戏核心
        /// </summary>
        /// <param name="frameIntervalSeconds">帧间隔时间(单位/秒)</param>
        /// <param name="catchedFrameDataCount">缓存的帧数据数量</param>
        public GameCore(Number frameInterval, int frameDataCacheLimit = 10)
        {
            runtime = new RuntimeModel(frameInterval, frameDataCacheLimit);
            runtime.SystemStart();
        }

        public void Update(FrameInput frameInput)
        {
            //增加新的一帧
            runtime.IncreaseFrame(frameInput);

            //系统更新
            PerformanceAnalysis.BeginAnalysis("GameCore_Update:SystemUpdate");
            runtime.SystemUpdate();
            PerformanceAnalysis.EndAnalysis("GameCore_Update:SystemUpdate");

            //固化帧数据
            PerformanceAnalysis.BeginAnalysis("GameCore_Update:ConfirmFrameData");
            runtime.ConfirmFrameData();
            PerformanceAnalysis.EndAnalysis("GameCore_Update:ConfirmFrameData");

            //缓存帧数据
            runtime.CacheFrameData();          
        }

        /// <summary>
        /// 结束GameCore生命周期
        /// </summary>
        public void Close()
        {
            runtime.SystemEnd();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
