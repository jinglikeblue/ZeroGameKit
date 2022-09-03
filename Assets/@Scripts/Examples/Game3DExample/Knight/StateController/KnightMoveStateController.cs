using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Knight
{
    public class KnightMoveStateController : BaseKnightStateController
    {
        public KnightMoveStateController(Knight knight) : base(knight, EKnightState.MOVE)
        {
        }

        public override bool CheckSwitchEnable(EKnightState fromState, object data)
        {
            return true;
        }

        public override void OnEnter(EKnightState fromState, object data)
        {

        }

        public override void OnExit(EKnightState toState, object data)
        {
            CheckMoveState();
        }

        public override void OnUpdate(EKnightState currentState, object data)
        {
            if (Knight.VO.speed == 0)
            {
                Knight.FSM.SwitchState(EKnightState.IDLE);
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

            CheckMoveState();

            Vector3 dir = Knight.VO.moveDir.normalized;
            Knight.CC.SimpleMove(dir * Knight.VO.speed);

            Rotation();
        }

        void Rotation()
        {
            if (Knight.VO.moveDir != Vector3.zero)
            {
                Quaternion q = Quaternion.FromToRotation(Vector3.forward, Knight.VO.moveDir);
                Vector3 angle = new Vector3(0, q.eulerAngles.y, 0);
                if (Knight.gameObject.transform.eulerAngles != angle)
                {
                    Knight.gameObject.transform.DORotate(angle, 0.3f);
                }
            }
        }

        void CheckMoveState()
        {
            var stateInfo = Knight.ASM.Animator.GetCurrentAnimatorStateInfo(0);
            if(Knight.VO.speed >= 5 && stateInfo.fullPathHash != Knight.ASM.RunHash)
            {
                Knight.ASM.SyncParameters();
            }
            else if(Knight.VO.speed >= 2 && stateInfo.fullPathHash != Knight.ASM.WalkHash)
            {
                Knight.ASM.SyncParameters();
            }
        }
    }
}
