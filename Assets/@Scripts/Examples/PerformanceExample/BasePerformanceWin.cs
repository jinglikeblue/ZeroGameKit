using System;
using System.Diagnostics;
using UnityEngine.UI;
using ZeroGameKit;

namespace Example
{
    /// <summary>
    /// 性能测试窗口的基类
    /// </summary>
    public class BasePerformanceWin : WithCloseButtonWin
    {
        public InputField inputFieldCount;
        public Text textResult;
        public Button btnCalculate;

        Stopwatch _watcher = new Stopwatch();

        protected override void OnEnable()
        {
            base.OnEnable();
            btnCalculate.onClick.AddListener(OnBtnCalculateClick);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            btnCalculate.onClick.RemoveListener(OnBtnCalculateClick);
        }

        void OnBtnCalculateClick()
        {
            int count = ReadInputCount();
            if (count > 0)
            {
                StartTest(count);
            }
        }

        /// <summary>
        /// 开始测试
        /// </summary>
        protected virtual void StartTest(int count)
        {
            _watcher.Restart();
            btnCalculate.enabled = false;
        }

        protected void EndTest()
        {
            _watcher.Stop();

            var output = $"执行耗时：{_watcher.ElapsedMilliseconds} 毫秒";
            SetOutput(output);
            btnCalculate.enabled = true;
        }

        protected void SetOutput(string output)
        {
            UnityEngine.Debug.Log(output);

            textResult.text = output;
        }

        /// <summary>
        /// 以数字形式读取输入的数
        /// </summary>
        /// <returns></returns>
        protected int ReadInputCount()
        {
            int count;  
            
            try
            {
                count = int.Parse(inputFieldCount.text);
            }
            catch
            {
                count = 0;
                SetOutput("填入的内容有误");
            }

            return count;
        }
    }
}
