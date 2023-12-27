using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPong
{
    /// <summary>
    /// 帧输入数据
    /// </summary>
    public struct FrameInput
    {
        /// <summary>
        /// 默认值
        /// </summary>
        public static FrameInput Default
        {
            get
            {
                FrameInput input = new FrameInput();
                input.playerInputs = new PlayerInput[2];
                for(int i = 0; i < input.playerInputs.Length; i++)
                {
                    input.playerInputs[i] = PlayerInput.Default;
                }
                return input;
            }
        }

        /// <summary>
        /// 玩家输入数据
        /// </summary>
        public PlayerInput[] playerInputs;
    }
}
