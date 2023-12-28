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

                if(playerInput.moveDir == EMoveDir.NONE)
                {
                    continue;
                }

                int speedDir = playerInput.moveDir == EMoveDir.LEFT ? -1 : 1;
                var moveVector = speedDir * runtime.vo.frameInterval * playerInput.moveSpeed;

                PlayerEntity playerEntity = runtime.vo.runtimeFrameData.world.players[i];
                playerEntity.position.x += moveVector;
                EnsurePlayerPositionProperly(playerEntity, runtime.vo.runtimeFrameData.world);
            }

            //移动球
            BallEntity ballEntity = runtime.vo.runtimeFrameData.world.ball;
            ballEntity.position.x += ballEntity.speed.x * runtime.vo.frameInterval;
            ballEntity.position.y += ballEntity.speed.y * runtime.vo.frameInterval;
            EnsureBallPositionProperly(ballEntity, runtime.vo.runtimeFrameData.world);
        }

        static void EnsurePlayerPositionProperly(PlayerEntity playerEntity, WorldEntity worldEntity)
        {
            
        }

        static void EnsureBallPositionProperly(BallEntity ballEntity, WorldEntity worldEntity)
        {

        }
    }
}
