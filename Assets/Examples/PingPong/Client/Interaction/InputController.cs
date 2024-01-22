using UnityEngine;

namespace PingPong
{
    /// <summary>
    /// 输入控制器
    /// </summary>
    public class InputController
    {
        object _threadLock = new object();

        /// <summary>
        /// 玩家输入缓存
        /// </summary>
        PlayerInput _playerInputCache = PlayerInput.Default;

        /// <summary>
        /// 采集用户输入
        /// </summary>
        public void CollectInput()
        {
            lock (_threadLock)
            {
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    _playerInputCache.moveDir = EMoveDir.LEFT;
                }
                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    _playerInputCache.moveDir = EMoveDir.RIGHT;
                }
                else
                {
                    _playerInputCache.moveDir = EMoveDir.NONE;
                }
            }
        }

        /// <summary>
        /// 提取用户输入
        /// </summary>
        public PlayerInput ExtractInput()
        {            
            lock (_threadLock)
            {
                return _playerInputCache;
            }
        }
    }
}
