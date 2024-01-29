namespace PingPong
{
    public class Protocols
    {
        #region Client To Server

        /// <summary>
        /// 加入主机
        /// </summary>
        public struct JoinHostRequest
        {

        }

        /// <summary>
        /// 游戏准备好了
        /// </summary>
        public struct GameReadyRequest
        {

        }

        /// <summary>
        /// 玩家输入
        /// </summary>
        public struct InputRequest
        {

        }

        #endregion

        #region Server To Client

        /// <summary>
        /// 游戏开始
        /// </summary>
        public struct GameStartNotify
        {

        }

        /// <summary>
        /// 帧输入数据同步
        /// </summary>
        public struct FrameInputNotify
        {

        }

        #endregion


    }
}
