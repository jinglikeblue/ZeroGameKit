using Jing.FixedPointNumber;
using System;

namespace PingPong
{
    /// <summary>
    /// 帧数据
    /// </summary>
    public struct FrameData : ISerializable<FrameData>, ILog
    {
        /// <summary>
        /// 经过了的帧数
        /// </summary>
        public ulong elapsedFrames;

        /// <summary>
        /// 经过了的时间，单位/秒
        /// </summary>
        public Number elapsedTime;

        /// <summary>
        /// 输入数据
        /// </summary>
        public FrameInput input;

        /// <summary>
        /// 世界的数据
        /// </summary>
        public WorldEntity world;

        public FrameData Clone()
        {            
            return CopyUtility.DeepCopy(this);
        }

        public void Deserialize(byte[] data)
        {
            throw new NotImplementedException();
        }

        public byte[] Serialize()
        {
            throw new NotImplementedException();
        }

        public string ToLog()
        {
            throw new NotImplementedException();
        }
    }
}
