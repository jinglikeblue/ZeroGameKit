using System;
using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using ZeroHot;

namespace Sokoban
{
    class MoveableUnit : BaseUnit
    {       
        enum EState
        {
            IDLE,
            MOVE
        }

        FiniteStateMachine<EState> _fsm = new FiniteStateMachine<EState>();
        public Vector2Int TileLastAt { get; private set; }
        public Vector2Int TileMoveTo { get; private set; }        
        public bool IsMoving { get; private set; }
        public EDir MoveDir { get; private set; }
        /// <summary>
        /// 移动速度
        /// </summary>
        public float moveSpeed = 2f;

        Vector3 _targetLocalPosition;

        public event Action<MoveableUnit> onMoveEnd;
        public event Action<MoveableUnit> onMoveStart;
        

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            _fsm.RegistState(EState.IDLE, OnEnterState);
            _fsm.RegistState(EState.MOVE, OnEnterState, null, OnUpdateMoveState);
            _fsm.SwitchState(EState.IDLE);
        }

        private void OnEnterState(EState nowState, object data)
        {
            switch (_fsm.CurrentState)
            {
                case EState.IDLE:
                    IsMoving = false;                    
                    break;
                case EState.MOVE:
                    IsMoving = true;
                    onMoveStart?.Invoke(this);
                    break;
            }
        }

        private void OnUpdateMoveState(EState nowState, object data)
        {
            gameObject.transform.localPosition = Vector3.MoveTowards(gameObject.transform.localPosition, _targetLocalPosition, Time.deltaTime * moveSpeed);
            if (_targetLocalPosition == gameObject.transform.localPosition)
            {
                Tile = TileMoveTo;             
                _fsm.SwitchState(EState.IDLE);
                onMoveEnd?.Invoke(this);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            ILBridge.Ins.onUpdate += OnUpdate;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ILBridge.Ins.onUpdate -= OnUpdate;
        }

        private void OnUpdate()
        {
            _fsm.Update();
        }

        public virtual bool Move(EDir dir, Vector2Int targetTile)
        {
            if (IsMoving)
            {
                return false;
            }

            TileLastAt = Tile;

            TileMoveTo = targetTile;

            MoveDir = dir;

            _targetLocalPosition = TileUtil.Tile2Position(TileMoveTo);
            _fsm.SwitchState(EState.MOVE);
            return true;
        } 
    }
}
