using UnityEngine;
using Zero;
using Jing;
using System.Collections.Generic;
using Zero;

namespace Knight
{
    public class Knight : AView
    {
        /// <summary>
        /// 移动的速度
        /// </summary>
        const float MOVE_SPEED = 2;

        /// <summary>
        /// 跑动的速度
        /// </summary>
        const float RUN_SPEED = 5;

        public KnightASM ASM { get; private set; }
        public KnightVO VO { get; private set; }
        public CharacterController CC { get; private set; }
        public FiniteStateMachine<EKnightState> FSM { get; private set; }

        Dictionary<EKnightState, BaseKnightStateController> _stateDic = new Dictionary<EKnightState, BaseKnightStateController>();

        protected override void OnInit(object data)
        {
            VO = new KnightVO();
            FSM = new FiniteStateMachine<EKnightState>();
            ASM = new KnightASM(GetComponent<Animator>(), VO);            
            CC = GetComponent<CharacterController>();

            _stateDic[EKnightState.IDLE] = new KnightIdleStateController(this);
            _stateDic[EKnightState.MOVE] = new KnightMoveStateController(this);
            _stateDic[EKnightState.ATTACK] = new KnightAttackStateController(this);
            _stateDic[EKnightState.DEFENCE] = new KnightDefenceStateController(this);
            FSM.SwitchState(EKnightState.IDLE);
        }

        protected override void OnEnable()
        {
            ILBridge.Ins.onUpdate += OnUpdate;
        }

        protected override void OnDisable()
        {
            ILBridge.Ins.onUpdate -= OnUpdate;
        }

        private void OnUpdate()
        {
            FSM.Update(Time.deltaTime);            
        }

        public void Move(Vector3 dir)
        {
            VO.moveDir = dir;
            UpdateMoveSpeed();
        }

        void UpdateMoveSpeed()
        {
            if (VO.moveDir != Vector3.zero)
            {
                if (VO.moveDir.magnitude > 0.9f)
                {
                    VO.speed = RUN_SPEED;
                }
                else
                {
                    VO.speed = MOVE_SPEED;
                }
            }
            else
            {
                VO.speed = 0;
            }                     
        }

        public void Attack(bool isHold)
        {
            if (isHold)
            {
                VO.action = 2;
            }
            else
            {
                VO.action = 0;
            }                        
        }

        public void Block(bool isHold)
        {
            if (isHold)
            {
                VO.action = 1;
            }
            else
            {
                VO.action = 0;
            }
        }
    }
}