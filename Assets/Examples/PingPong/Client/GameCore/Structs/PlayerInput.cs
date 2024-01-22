using Jing.FixedPointNumber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPong
{
    /// <summary>
    /// 输入
    /// </summary>
    public struct PlayerInput
    {
        /// <summary>
        /// 默认值
        /// </summary>
        public static PlayerInput Default
        {
            get
            {
                return new PlayerInput(EMoveDir.NONE, Define.PLAYER_MOVE_SPEED);
            }
        }

        /// <summary>
        /// 移动方向
        /// </summary>
        public EMoveDir moveDir;

        /// <summary>
        /// 移动速度
        /// </summary>
        public Number moveSpeed;

        public PlayerInput(EMoveDir moveDir, Number moveSpeed)
        {
            this.moveDir = moveDir;
            this.moveSpeed = moveSpeed;
        }
    }
}
