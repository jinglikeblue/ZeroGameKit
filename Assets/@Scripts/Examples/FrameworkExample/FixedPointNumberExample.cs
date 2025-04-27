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
using Zero;

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
            
            var fixedA = Number.CreateFromDouble(a);
            var fixedB = Number.CreateFromDouble(b);

            L($"定点数最大值: {Number.MAX_VALUE}");
            L($"定点数最小值: {Number.MIN_VALUE}");
            L($"定点数小数范围: [0,{Number.FRACTION_MASK}]");

            L($"输入的数字:");
            L($"a = {a} [整数部分: {fixedA.IntegerPart} 小数部分：{fixedA.FractionalPart}] Raw: {fixedA.ToBinary(false)}]");
            L($"b = {b} [Binary: {fixedB.IntegerPart}.{fixedB.FractionalPart}] Raw: {fixedB.ToBinary(false)}]");
            L("");

            var n1 = new Number(4, 10000);
            var n2 = new Number(4, 1000);
            var n3 = n1 * 1000;
            Debug.Log($"N1: {n1.Info}");
            Debug.Log($"N2: {n2.Info}");
            Debug.Log($"N3: {n3.Info}");

            var integerPart = fixedA.IntegerPart.ToBinary(true);
            var fractionalPart = fixedA.FractionalPart.ToBinary(true);
            Debug.Log($"整数部分二进制: {integerPart}");
            Debug.Log($"小数部分二进制: {fractionalPart}");
            Debug.Log($"完整数据二进制: {fixedA.ToBinary(true)}");
            Debug.Log($"完整数据二进制: {fixedA.IntegerPart.ToBinary(false)}.{fixedA.FractionalPart.ToBinary(false)}");
            
            
            L($"浮点数运算：");
            L($"a + b = {a + b}");
            L($"a - b = {a - b}");
            L($"a * b = {a * b}");
            L($"a / b = {a / b}");
            L("");

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
