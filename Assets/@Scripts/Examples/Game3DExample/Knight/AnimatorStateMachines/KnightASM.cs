using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zero;

namespace Knight
{
    public class KnightASM
    {
        const string SPEED = "speed";
        const string DEATH_TYPE = "deathType";
        const string ACTION = "action";
        const string IDLE_TYPE = "idleType";

        public int ActionTagHash { get; }

        public int Idle1Hash { get; }

        public int WalkHash { get; }

        public int RunHash { get; }

        public int AttackHash { get; }

        public int BlockIdleHash { get; }
        public int BlockHash { get; }

        public Animator Animator { get; }

        KnightVO _vo;

        public KnightASM(Animator animator, KnightVO vo)
        {
            Animator = animator;
            _vo = vo;

            ActionTagHash = Animator.StringToHash("Action");
            Idle1Hash = Animator.StringToHash("Base Layer.Idle.Idle1");
            WalkHash = Animator.StringToHash("Base Layer.Move.Walk");
            RunHash = Animator.StringToHash("Base Layer.Move.Run");
            AttackHash = Animator.StringToHash("Base Layer.Action.Attack");
            BlockIdleHash = Animator.StringToHash("Base Layer.Action.BlockIdle");
            BlockHash = Animator.StringToHash("Base Layer.Action.Block");
        }

        public void SyncParameters()
        {
            Animator.SetFloat(SPEED, _vo.speed);
            Animator.SetInteger(DEATH_TYPE, _vo.deathType);
            Animator.SetInteger(ACTION, _vo.action);
            Animator.SetInteger(IDLE_TYPE, _vo.idleType);
        }        
    }
}