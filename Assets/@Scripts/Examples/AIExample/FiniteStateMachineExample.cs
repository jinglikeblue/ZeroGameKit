using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ZeroGameKit;
using ZeroHot;

namespace Example
{
    class FiniteStateMachineExample
    {
        static public void Start()
        {
            var sb = new StringBuilder();
            new TestFSM(sb);
            var msg = MsgWin.Show("FiniteStateMachine", sb.ToString());
            msg.SetContentAlignment(UnityEngine.TextAnchor.UpperLeft);
        }
    }

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
}
