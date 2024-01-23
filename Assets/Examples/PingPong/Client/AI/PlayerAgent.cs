//using System;
using Jing.FixedPointNumber;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPong
{
    /// <summary>
    /// 角色代理
    /// </summary>
    public class PlayerAgent
    {
        int _playerIndex;

        public int PlayerIndex => _playerIndex;

        PlayerInput _playerInput = PlayerInput.Default;

        public PlayerAgent(int playerIndex)
        {
            _playerIndex = playerIndex;            
        }

        public void Update(GameCore gameCore)
        {
            var player = gameCore.FrameData.world.players[_playerIndex];
            var ball = gameCore.FrameData.world.ball;
            var distance = ball.position.x - player.position.x;
            if(distance > 1)
            {
                _playerInput.moveDir = _playerIndex == 0 ? EMoveDir.LEFT : EMoveDir.RIGHT;
            }
            else if(distance < -1)
            {
                _playerInput.moveDir = _playerIndex == 0 ? EMoveDir.RIGHT : EMoveDir.LEFT;
            }
            else
            {
                _playerInput.moveDir = EMoveDir.NONE;
            }
        }

        /// <summary>
        /// 获取角色的输入
        /// </summary>
        /// <returns></returns>
        public PlayerInput GetInput()
        {
            return _playerInput;
        }
    }
}
