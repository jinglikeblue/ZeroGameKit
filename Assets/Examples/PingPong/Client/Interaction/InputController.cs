using Jing.FixedPointNumber;
using System.Collections.Generic;
using UnityEngine;
using Zero;

namespace PingPong
{
    /// <summary>
    /// 输入控制器
    /// </summary>
    public class InputController
    {
        object _threadLock = new object();

        Dictionary<int, PlayerInput> _playerInputDic = new Dictionary<int, PlayerInput>();

        Number _moveCoefficient = 0;
        public void SetMoveCoefficient(Number value)
        {
            _moveCoefficient = value;
        }

        /// <summary>
        /// 采集用户输入
        /// </summary>
        public void CollectInput(GameCore gameCore)
        {
            lock (_threadLock)
            {
                PlayerInput playerInput = PlayerInput.Default;
                if (_moveCoefficient != 0)
                {
                    //TODO 通过Define.WROLD_SIZE算出Player限定的位置。然后如果Player超出了这个位置，则不移动了
                    var limitPos = Define.WORLD_SIZE.HalfX * _moveCoefficient;
                    var playerPos = gameCore.FrameData.world.players[0].position.x;
                    GUIDebugInfo.SetInfo("Move Pos", $"Limit:{limitPos},Player:{playerPos}");

                    #region 检测触摸输入
                    if (_moveCoefficient < 0)
                    {
                        //if (playerPos > limitPos)
                        //{
                            playerInput.moveDir = EMoveDir.LEFT;
                        //}
                    }
                    else
                    {
                        //if (playerPos < limitPos)
                        //{
                            playerInput.moveDir = EMoveDir.RIGHT;
                        //}
                    }
                    #endregion
                }
                else
                {
                    #region 检测键盘输入                    
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
                    #endregion
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
