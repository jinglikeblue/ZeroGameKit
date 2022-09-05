using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Knight
{
    public class KnightIdleStateController : BaseKnightStateController
    {
        float idleTypeSwitchCD = 5f;
        public KnightIdleStateController(Knight knight) : base(knight, EKnightState.IDLE)
        {
        }

        public override bool CheckSwitchEnable(EKnightState fromState, object data)
        {
            return true;
        }

        public override void OnEnter(EKnightState fromState, object data)
        {
            Knight.VO.idleType = 0;
            Knight.ASM.SyncParameters();
        }

        public override void OnExit(EKnightState toState, object data)
        {
            
        }

        public override void OnUpdate(EKnightState currentState, object data)
        {
            if(Knight.VO.speed > 0)
            {
                Knight.FSM.SwitchState(EKnightState.MOVE);
                return;
            }

            switch (Knight.VO.action)
            {
                case 1:
                    Knight.FSM.SwitchState(EKnightState.DEFENCE);
                    return;
                case 2:
                    Knight.FSM.SwitchState(EKnightState.ATTACK);
                    return;
            }

            
            //如果是空闲状态每隔几秒随机切换一个特殊的“打发时间”动画
            var stateInfo = Knight.ASM.Animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.fullPathHash == Knight.ASM.Idle1Hash)
            {
                idleTypeSwitchCD -= Time.deltaTime;
                if (idleTypeSwitchCD <= 0)
                {
                    var random = UnityEngine.Random.Range(0, 6);
                    switch (random)
                    {
                        case 2:
                            Knight.VO.idleType = 2;
                            Knight.ASM.SyncParameters();
                            break;
                        case 3:
                            Knight.VO.idleType = 3;
                            Knight.ASM.SyncParameters();
                            break;
                        case 4:
                            Knight.VO.idleType = 4;
                            Knight.ASM.SyncParameters();
                            break;
                    }
                    idleTypeSwitchCD = 5f;
                }
            }
        }
    }
}
