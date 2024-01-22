using Jing.FixedPointNumber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingPong
{
    /// <summary>
    /// 移动系统
    /// </summary>
    public static class MoveSystem
    {
        public static void Update(RuntimeModel runtime)
        {
            //移动角色
            var playerInputs = runtime.vo.runtimeFrameData.input.playerInputs;
            for (int i = 0; i < playerInputs.Length; i++)
            {
                PlayerInput playerInput = playerInputs[i];

                PlayerEntity playerEntity = runtime.vo.runtimeFrameData.world.players[i];

                if (playerInput.moveDir == EMoveDir.NONE)
                {
                    playerEntity.speed = Number.ZERO;
                    continue;
                }

                int speedDir = playerInput.moveDir == EMoveDir.LEFT ? -1 : 1;
                playerEntity.speed = speedDir * playerInput.moveSpeed;
                var moveVector = playerEntity.speed * runtime.vo.frameInterval;                
                playerEntity.position.x += moveVector;
                EnsurePlayerPositionProperly(playerEntity, runtime.vo.runtimeFrameData.world);
            }

            //移动球
            BallEntity ballEntity = runtime.vo.runtimeFrameData.world.ball;
            ballEntity.position.x += ballEntity.speed.x * runtime.vo.frameInterval;
            ballEntity.position.y += ballEntity.speed.y * runtime.vo.frameInterval;
            EnsureBallPositionProperly(ballEntity, runtime.vo.runtimeFrameData.world);
        }

        /// <summary>
        /// 计算球的位置
        /// </summary>
        /// <param name="ballEntity"></param>
        /// <param name="pastTime"></param>
        /// <returns></returns>
        public static Vector2 CalculateBallPosition(BallEntity ballEntity, Number pastTime)
        {
            var moveVector = ballEntity.speed * pastTime;
            return ballEntity.position + moveVector;
        }

        /// <summary>
        /// 计算球员的位置
        /// </summary>
        /// <param name="playerEntity"></param>
        /// <param name="pastTime"></param>
        /// <returns></returns>
        public static Number CalculatePlayerPosition(PlayerEntity playerEntity, Number pastTime)
        {
            var moveVector = playerEntity.speed * pastTime;
            return playerEntity.position.x + moveVector;
        }

        /// <summary>
        /// 确保玩家位置是在合理范围内
        /// </summary>
        /// <param name="playerEntity"></param>
        /// <param name="worldEntity"></param>
        static void EnsurePlayerPositionProperly(PlayerEntity playerEntity, WorldEntity worldEntity)
        {
            var playerRect = PlayerUtility.CreatePlayerBody(playerEntity);
            var worldRect = worldEntity.size;

            var leftDistance = worldRect.left - playerRect.left;
            if(leftDistance > 0)
            {
                //说明超出左边边界了
                playerEntity.position.x = worldRect.left + playerRect.width / 2;
                return;
            }

            var rightDistance = playerRect.right - worldRect.right;
            if(rightDistance > 0)
            {
                //说明超出右边边界了
                playerEntity.position.x = worldRect.right - playerRect.width / 2;
                return;
            }
        }

        /// <summary>
        /// 确保球的位置是在合理范围内
        /// </summary>
        /// <param name="ballEntity"></param>
        /// <param name="worldEntity"></param>
        static void EnsureBallPositionProperly(BallEntity ballEntity, WorldEntity worldEntity)
        {
            var ballPosX = ballEntity.position.x;
            var ballPosY = ballEntity.position.y;
            var worldRect = worldEntity.size;

            var leftDistance = worldRect.left - ballPosX;
            if (leftDistance > 0)
            {
                //说明超出左边边界了
                ballEntity.position.x = worldRect.left;

                //反弹变向
                ballEntity.speed.x *= -1;
                return;
            }

            var rightDistance = ballPosX - worldRect.right;
            if (rightDistance > 0)
            {
                //说明超出右边边界了
                ballEntity.position.x = worldRect.right;

                //反弹变向
                ballEntity.speed.x *= -1;
                return;
            }
        }
    }
}
