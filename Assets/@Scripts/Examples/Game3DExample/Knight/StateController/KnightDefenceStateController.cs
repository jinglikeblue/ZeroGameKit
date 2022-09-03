using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knight
{
    public class KnightDefenceStateController : BaseKnightStateController
    {
        public KnightDefenceStateController(Knight knight) : base(knight, EKnightState.DEFENCE)
        {
        }

        public override bool CheckSwitchEnable(EKnightState fromState, object data)
        {
            return true;
        }

        public override void OnEnter(EKnightState fromState, object data)
        {
            Knight.ASM.SyncParameters();
        }

        public override void OnExit(EKnightState toState, object data)
        {

        }

        public override void OnUpdate(EKnightState currentState, object data)
        {
            var stateInfo = Knight.ASM.Animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.fullPathHash == Knight.ASM.BlockIdleHash &&  Knight.VO.isBlock)
            {
                Knight.ASM.SyncParameters();                
                return;
            }            

            if (Knight.VO.action == 0)
            {
                if (Knight.VO.speed > 0)
                {
                    Knight.FSM.SwitchState(EKnightState.MOVE);
                }
                else
                {
                    Knight.FSM.SwitchState(EKnightState.IDLE);
                }
            }
            else if(Knight.VO.action == 2)
            {
                Knight.FSM.SwitchState(EKnightState.ATTACK);
            }
        }
    }
}
