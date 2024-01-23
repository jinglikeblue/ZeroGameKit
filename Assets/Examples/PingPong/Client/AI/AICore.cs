using System.Collections.Generic;

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

        List<PlayerAgent> _agentList;

        /// <summary>
        /// AI核心初始化
        /// </summary>
        public void Init(int[] playerIndexs)
        {
            _agentList = new List<PlayerAgent>();
            foreach (var playerIndex in playerIndexs)
            {
                _agentList.Add(new PlayerAgent(playerIndex));
            }
        }

        /// <summary>
        /// 更新AI核心
        /// </summary>
        /// <param name="gameCore"></param>
        /// <returns>返回是否AI核心进行了更新</returns>
        public bool Update(GameCore gameCore)
        {
            if (_lastUpdateFrame == gameCore.FrameData.elapsedFrames)
            {
                return false;
            }
            _lastUpdateFrame = gameCore.FrameData.elapsedFrames;
            foreach (var agent in _agentList)
            {
                agent.Update(gameCore);
            }
            return true;
        }

        public PlayerAgent[] GetAgents()
        {
            return _agentList?.ToArray();
        }
    }
}
