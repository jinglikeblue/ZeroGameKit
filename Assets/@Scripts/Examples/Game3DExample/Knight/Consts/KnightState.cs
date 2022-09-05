using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knight
{
    public enum EKnightState
    {
        /// <summary>
        /// 空闲
        /// </summary>
        IDLE,
        /// <summary>
        /// 移动
        /// </summary>
        MOVE,
        /// <summary>
        /// 攻击
        /// </summary>
        ATTACK,
        /// <summary>
        /// 防御
        /// </summary>
        DEFENCE,
        
    }
}
