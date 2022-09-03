using UnityEngine;

namespace Knight
{
    public class KnightVO
    {
        /// <summary>
        /// 移动方向
        /// </summary>
        public Vector3 moveDir = Vector3.zero;

        /// <summary>
        /// 移动速度
        /// </summary>
        public float speed = 0;

        /// <summary>
        /// 死亡类型
        /// </summary>
        public int deathType = 0;

        /// <summary>
        /// 动作类型
        /// </summary>
        public int action = 0;

        /// <summary>
        /// 空闲类型
        /// </summary>
        public int idleType = 0;

        /// <summary>
        /// 是否在格挡
        /// </summary>
        public bool isBlock = false;
    }
}
