using Jing;
using Jing.FixedPointNumber;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using ZeroGameKit;
using ZeroHot;

namespace Example
{
    class FixedPointNumberExample
    {
        static public void Start()
        {
            UIWinMgr.Ins.Open<FixedPointNumberExampleWin>();
        }
    }

    class FixedPointNumberExampleWin : WithCloseButtonWin
    {
        public Button btnCalculate;
        public InputField textInputA;
        public InputField textInputB;

        public Text textLog;
        public void L(string v)
        {
            textLog.text += "\r\n" + v;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            btnCalculate.onClick.AddListener(Calculate);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            btnCalculate.onClick.RemoveListener(Calculate);
        }

        private void Calculate()
        {
            textLog.text = "";
            double a;
            double b;

            try
            {
                a = double.Parse(textInputA.text);
                b = double.Parse(textInputB.text);
            }
            catch (Exception e)
            {
                L($"输入的内容不对 a:{textInputA.text}   b:{textInputB.text}");
                return;
            }


            L($"输入的数字:");
            L($"a = {a}");
            L($"b = {b}");
            L("");


            L($"浮点数运算：");
            L($"a + b = {a + b}");
            L($"a - b = {a - b}");
            L($"a * b = {a * b}");
            L($"a / b = {a / b}");
            L("");


            var fixedA = Number.CreateFromDouble(a);
            var fixedB = Number.CreateFromDouble(b);
            L($"定点数运算：");
            L($"{fixedA.Info}");
            L($"{fixedB.Info}");
            L($"a + b = {fixedA + fixedB}");
            L($"a - b = {fixedA - fixedB}");
            L($"a * b = {fixedA * fixedB}");
            L($"a / b = {fixedA / fixedB}");
            L("");

            L($"定点数位移运算：");            
            L($"a << 1 = {fixedA << 1}");
            L($"a >> 1 = {fixedA >> 1}");
            L($"b << 1 = {fixedB << 1}");
            L($"b >> 1 = {fixedB >> 1}");
        }
    }
}
