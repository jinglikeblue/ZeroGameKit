using Jing.FixedPointNumber;
using System.Collections.Generic;

namespace PingPong
{
    public class GameCore
    {
        /// <summary>
        /// 帧每次更新的时间间隔
        /// </summary>
        public Number frameInterval { get; private set; }

        /// <summary>
        /// 运行时的帧数据
        /// </summary>
        FrameData _runtimeFrameData;

        /// <summary>
        /// 帧数据缓存数量限制
        /// </summary>
        int _catchedFrameDataLimit;

        /// <summary>
        /// 缓存的帧数据
        /// </summary>
        LinkedList<FrameData> _cachedFrameDatas;

        /// <summary>
        /// 最新的一帧的数据
        /// </summary>
        public FrameData lastFrameData { get; private set; }

        /// <summary>
        /// 初始化游戏核心
        /// </summary>
        /// <param name="frameIntervalSeconds">帧间隔时间(单位/秒)</param>
        /// <param name="catchedFrameDataCount">缓存的帧数据数量</param>
        public void Init(Number frameInterval, int catchedFrameDataLimit = 10)
        {
            this.frameInterval = frameInterval;
            _runtimeFrameData = new FrameData();
            _runtimeFrameData.elapsedFrames = 0;
            _runtimeFrameData.elapsedTime = Number.ZERO;

            _catchedFrameDataLimit = catchedFrameDataLimit;
            _cachedFrameDatas = new LinkedList<FrameData>();
        }

        public void Update(FrameInput frameInput)
        {
            _runtimeFrameData.elapsedFrames += 1;
            _runtimeFrameData.elapsedTime += frameInterval;
            _runtimeFrameData.input = frameInput;

            #region 逻辑更新

            #endregion

            #region 缓存帧数据
            lastFrameData = _runtimeFrameData.Clone();
            _cachedFrameDatas.AddLast(lastFrameData);
            if(_cachedFrameDatas.Count > _catchedFrameDataLimit)
            {
                _cachedFrameDatas.RemoveFirst();
            }
            #endregion
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
