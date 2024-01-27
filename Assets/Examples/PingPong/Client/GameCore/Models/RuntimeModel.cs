using Jing.FixedPointNumber;
using System.Collections.Generic;

namespace PingPong
{
    /// <summary>
    /// 运行时数据模型
    /// </summary>
    public class RuntimeModel
    {
        public RuntimeVO vo { get; private set; }

        public RuntimeModel(RuntimeVO vo)
        {
            this.vo = vo;
        }

        public RuntimeModel(Number frameInterval, int frameDataCacheLimit = 10)
        {
            vo = new RuntimeVO();
            vo.frameInterval = frameInterval;

            var frameData = new FrameData();
            frameData.elapsedFrames = 0;
            frameData.elapsedTime = Number.ZERO;
            vo.runtimeFrameData = frameData;

            vo.frameDataCacheLimit = frameDataCacheLimit;
            vo.cachedFrameDatas = new LinkedList<FrameData>();
        }

        /// <summary>
        /// 增加新的一帧(进入新的一帧)
        /// </summary>
        public void IncreaseFrame(FrameInput frameInput)
        {
            vo.runtimeFrameData.elapsedFrames++;
            vo.runtimeFrameData.elapsedTime += vo.frameInterval;
            vo.runtimeFrameData.input = frameInput;
        }

        public void SystemStart()
        {
            CenterSystem.Start(this);
        }

        /// <summary>
        /// 系统更新
        /// </summary>
        public void SystemUpdate()
        {
            CenterSystem.Update(this);

            CenterSystem.LateUpdate(this);
        }

        public void SystemEnd()
        {
            CenterSystem.End(this);
        }

        /// <summary>
        /// 固化当前帧
        /// </summary>
        public void ConfirmFrameData()
        {
            vo.confirmedFrameData = vo.runtimeFrameData.Clone();
        }

        /// <summary>
        /// 缓存帧数据
        /// </summary>
        public void CacheFrameData()
        {
            if (vo.frameDataCacheLimit < 1)
            {
                return;
            }
            vo.cachedFrameDatas.AddLast(vo.confirmedFrameData);
            if (vo.cachedFrameDatas.Count > vo.frameDataCacheLimit)
            {
                vo.cachedFrameDatas.RemoveFirst();
            }
        }
    }
}
