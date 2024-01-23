namespace PingPong
{
    /// <summary>
    /// AI核心
    /// </summary>
    public class AICore
    {
        /// <summary>
        /// 最后一次更新时的GameCore帧数
        /// </summary>
        ulong _lastUpdateFrame = ulong.MaxValue;

        /// <summary>
        /// AI核心初始化
        /// </summary>
        public void Init()
        {

        }

        /// <summary>
        /// 更新AI核心
        /// </summary>
        /// <param name="gameCore"></param>
        /// <returns>返回是否AI核心进行了更新</returns>
        public bool Update(GameCore gameCore)
        {
            if(_lastUpdateFrame == gameCore.FrameData.elapsedFrames)
            {
                return false;
            }
            _lastUpdateFrame = gameCore.FrameData.elapsedFrames;

            return true;
        }

        public PlayerAgent[] GetAgents()
        {
            return null;
        }
    }
}
