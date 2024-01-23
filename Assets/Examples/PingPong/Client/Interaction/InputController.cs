using System.Collections.Generic;
using UnityEngine;

namespace PingPong
{
    /// <summary>
    /// 输入控制器
    /// </summary>
    public class InputController
    {
        object _threadLock = new object();

        Dictionary<int, PlayerInput> _playerInputDic = new Dictionary<int, PlayerInput>();

        /// <summary>
        /// 采集用户输入
        /// </summary>
        public void CollectInput()
        {
            lock (_threadLock)
            {
                PlayerInput playerInput = PlayerInput.Default;
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    playerInput.moveDir = EMoveDir.LEFT;
                }
                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    playerInput.moveDir = EMoveDir.RIGHT;
                }
                else
                {
                    playerInput.moveDir = EMoveDir.NONE;
                }
                _playerInputDic[0] = playerInput;
            }
        }

        /// <summary>
        /// 收集AI行为
        /// </summary>
        /// <param name="playerIndex"></param>
        /// <param name="input"></param>
        public void CollectAIBehavior(int playerIndex, PlayerInput input)
        {
            lock (_threadLock)
            {
                _playerInputDic[playerIndex] = input;
            }
        }

        /// <summary>
        /// 提取用户输入
        /// </summary>
        public PlayerInput[] ExtractInput()
        {
            lock (_threadLock)
            {
                PlayerInput[] inputs = new PlayerInput[_playerInputDic.Count];
                for (int i = 0; i < inputs.Length; i++)
                {
                    inputs[i] = _playerInputDic[i];
                }
                return inputs;
            }
        }
    }
}
