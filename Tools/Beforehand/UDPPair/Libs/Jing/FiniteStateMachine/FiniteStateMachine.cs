using System.Collections.Generic;

/* 示例代码
    public class TestFSM
    {
        public enum ET
        {
            A,
            B,
            C,
        }

        FiniteStateMachine<ET> fsm = new FiniteStateMachine<ET>();
        StringBuilder _sb;
        public TestFSM(StringBuilder sb)
        {
            _sb = sb;            
            fsm.RegistState(ET.A, OnEnter, OnExit, OnUpdate, SwitchEnable);
            fsm.RegistState(ET.B, OnEnter, OnExit, OnUpdate, SwitchEnable);
            fsm.RegistState(ET.C, OnEnter, OnExit, OnUpdate, SwitchEnable);

            Log($"注册状态机允许的切换规则： {ET.A} -> {ET.C}");
            fsm.AddSwitchRule(ET.A, ET.C);

            string logStart = $"尝试切换状态 Current：{fsm.CurrentState} To：{ET.A}";
            var result = fsm.SwitchState(ET.A, "ToA");
            Log($"{logStart} Result：{result}");
            fsm.Update(Time.deltaTime, "A?");

            logStart = $"尝试切换状态 Current：{fsm.CurrentState} To：{ET.B}";
            result = fsm.SwitchState(ET.B, "ToB");
            Log($"{logStart} Result：{result}");
            fsm.Update(Time.deltaTime, "B?");

            logStart = $"尝试切换状态 Current：{fsm.CurrentState} To：{ET.C}";
            result = fsm.SwitchState(ET.C, "ToC");
            Log($"{logStart} Result：{result}");
            fsm.Update(Time.deltaTime, "C?");
        }

        private void OnUpdate(ET currentState, object data)
        {
            Log($"状态更新 State：{currentState} StayTime：{fsm.StateStayTime}  Data：{data}");
        }

        private bool SwitchEnable(ET toState, object data)
        {
            Log($"检查状态切换是否允许 From：{fsm.CurrentState} To：{toState}  Data：{data}");
            return true;
        }

        private void OnExit(ET toState, object data)
        {
            Log($"退出状态 Current：{fsm.CurrentState} To：{toState}  Data：{data}");
        }

        private void OnEnter(ET fromState, object data)
        {
            Log($"进入状态 From：{fromState} Current：{fsm.CurrentState}  Data：{data}");
        }

        void Log(string s)
        {
            _sb.AppendLine(s);
            _sb.AppendLine();
        }
    }
*/

namespace Jing
{
    /// <summary>
    /// 有限状态机
    /// </summary>
    public class FiniteStateMachine<T>
    {
        public delegate void EnterStateDelegate(T fromState, object data);

        public delegate void ExitStateDelegate(T toState, object data);

        public delegate void UpdateStateDelegate(T currentState, object data);

        public delegate bool CheckSwitchStateEnableDelegate(T toState, object data);

        /// <summary>
        /// 状态控制器
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        class StateController<TState>
        {
            /// <summary>
            /// 进入状态的委托
            /// </summary>
            /// <param name="fromState"></param>
            public EnterStateDelegate onEnter;

            /// <summary>
            /// 退出状态的委托
            /// </summary>
            /// <param name="toState"></param>
            public ExitStateDelegate onExit;

            /// <summary>
            /// 更新状态的委托
            /// <param name="curState"></param>
            /// </summary>
            public UpdateStateDelegate onUpdate;

            /// <summary>
            /// 切换状态检查的委托
            /// </summary>
            /// <param name="toState"></param>
            /// <returns></returns>
            public CheckSwitchStateEnableDelegate checkSwitchEnable;

            /// <summary>
            /// 状态
            /// </summary>
            public TState state;

            /// <summary>
            /// 配置的能切换到的状态，null表示不限制
            /// </summary>
            public HashSet<TState> ruleSwitch = null;

            public StateController(TState state)
            {
                this.state = state;
            }
        }

        /// <summary>
        /// 在当前状态下经过的时间(根据Update传入的dt值累计）
        /// </summary>
        public float StateStayTime { get; private set; }

        /// <summary>
        /// 当前状态
        /// </summary>
        public T CurrentState { get; private set; }

        /// <summary>
        /// 状态字典
        /// </summary>
        Dictionary<T, StateController<T>> _stateDic = new Dictionary<T, StateController<T>>();

        /// <summary>
        /// 状态机名称
        /// </summary>
        public string Name { get; private set; }

        public FiniteStateMachine(string name = null)
        {
            this.Name = name;
        }

        /// <summary>
        /// 注册一个状态，不适用的方法可以传递Null
        /// 注册的第一个状态，将会成为状态机的初始状态
        /// </summary>
        public void RegistState(T state, EnterStateDelegate onEnter = null, ExitStateDelegate onExit = null, UpdateStateDelegate onUpdate = null, CheckSwitchStateEnableDelegate checkSwitchEnable = null)
        {
            StateController<T> sc = new StateController<T>(state);
            sc.onEnter = onEnter;
            sc.onExit = onExit;
            sc.onUpdate = onUpdate;
            sc.checkSwitchEnable = checkSwitchEnable;

            if (null == CurrentState)
            {
                //设置为第一个状态
                CurrentState = state;
            }

            _stateDic[state] = sc;
        }

        /// <summary>
        /// 注销一个状态
        /// </summary>
        public void UnregistState(T state)
        {
            if (_stateDic.ContainsKey(state))
            {
                _stateDic.Remove(state);
            }

            if (CurrentState.Equals(state))
            {
                CurrentState = default(T);
            }
        }

        /// <summary>
        /// 添加一个合法的状态转换规则
        /// 如果一个规则都不添加，则注册的状态之间可以随意切换，可以通过「checkSwitchEnable」自行判断
        /// </summary>
        /// <param name="fromState"></param>
        /// <param name="toState"></param>
        public void AddSwitchRule(T fromState, T toState)
        {
            if (false == _stateDic.ContainsKey(fromState))
            {
                return;
            }


            if (null == _stateDic[fromState].ruleSwitch)
            {
                _stateDic[fromState].ruleSwitch = new HashSet<T>();
            }

            _stateDic[fromState].ruleSwitch.Add(toState);
        }

        /// <summary>
        /// 移除一个合法的状态转换规则
        /// </summary>
        /// <param name="fromState"></param>
        public void RemoveSwitchRule(T fromState, T toState)
        {
            if (false == _stateDic.ContainsKey(fromState) || null == _stateDic[fromState].ruleSwitch)
            {
                return;
            }

            _stateDic[fromState].ruleSwitch.Remove(toState);
        }

        /// <summary>
        /// 进入一个状态
        /// </summary>
        public bool SwitchState(T toState, object data = null)
        {
            if (false == _stateDic.ContainsKey(toState))
            {
                return false;
            }

            var oldSC = _stateDic[CurrentState];

            if (oldSC.ruleSwitch != null && false == oldSC.ruleSwitch.Contains(toState))
            {
                return false;
            }

            var newSC = _stateDic[toState];

            if (null != oldSC.checkSwitchEnable && false == oldSC.checkSwitchEnable.Invoke(toState, data))
            {
                return false;
            }

            if (null != oldSC.onExit)
            {
                oldSC.onExit.Invoke(toState, data);
            }
            CurrentState = toState;
            StateStayTime = 0;
            if (null != newSC.onEnter)
            {
                newSC.onEnter.Invoke(oldSC.state, data);
            }
            return true;
        }

        /// <summary>
        /// 状态更新
        /// </summary>
        /// <param name="dt">距离上次状态更新的间隔，如果传入，可以统计状态持续的时间</param>
        public void Update(float dt = 0f, object data = null)
        {
            StateStayTime += dt;
            var nowSC = _stateDic[CurrentState];
            if (null != nowSC.onUpdate)
            {
                nowSC.onUpdate.Invoke(CurrentState, data);
            }
        }
    }
}
