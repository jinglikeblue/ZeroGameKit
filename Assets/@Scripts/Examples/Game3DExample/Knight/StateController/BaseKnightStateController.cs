using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knight
{ 
    public abstract class BaseKnightStateController
    {
        protected Knight Knight { get; private set; }
        public BaseKnightStateController(Knight knight, EKnightState state)
        {
            Knight = knight;
            knight.FSM.RegistState(state, OnEnter, OnExit, OnUpdate, CheckSwitchEnable);
        }

        public abstract void OnEnter(EKnightState fromState, object data);

        public abstract void OnExit(EKnightState toState, object data);

        public abstract void OnUpdate(EKnightState currentState, object data);

        public abstract bool CheckSwitchEnable(EKnightState fromState, object data);
    }
}
