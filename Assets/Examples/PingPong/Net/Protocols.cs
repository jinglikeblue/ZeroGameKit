using Jing;

namespace PingPong
{
    public class Protocols
    {        
        /// <summary>
        /// 协议打包
        /// </summary>
        /// <returns></returns>
        public static byte[] Pack(object obj)
        {
            return MsgPacker.Pack(obj);
        }

        /// <summary>
        /// 协议解包
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T Unpack<T>(byte[] data)
        {
            return MsgPacker.Unpack<T>(data);
        }

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
            public byte moveDir;
        }

        /// <summary>
        /// Ping请求
        /// </summary>
        public struct PingC2S
        {
            public long clientUTC;
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
            public int frame;
            public InputRequest[] inputs;
        }

        /// <summary>
        /// Pong回复
        /// </summary>
        public struct PongS2C
        {
            public long clientUTC;
            public long serverUTC;
        }

        #endregion

    }
}
