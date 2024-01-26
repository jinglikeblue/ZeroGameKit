using System;
using UnityEngine.UI;
using ZeroGameKit;
using ZeroHot;

namespace PingPong
{
    /// <summary>
    /// PingPong控制窗口
    /// </summary>
    public class PingPongGameControlWin : AView
    {
        public event Action onContinueSelected;
        public event Action onRestartSelected;
        public event Action onExitSelected;

        /// <summary>
        /// 重启按钮
        /// </summary>
        public Button btnContinue;

        /// <summary>
        /// 重启按钮
        /// </summary>
        Button btnRestart;

        /// <summary>
        /// 退出按钮
        /// </summary>
        Button btnExit;

        Text textTitle;
        Text textOutput;

        protected override void OnEnable()
        {
            base.OnEnable();
            btnContinue.onClick.AddListener(OnClickContinue);
            btnRestart.onClick.AddListener(OnClickRestart);
            btnExit.onClick.AddListener(OnClickExit);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            btnContinue.onClick.RemoveListener(OnClickContinue);
            btnRestart.onClick.RemoveListener(OnClickRestart);
            btnExit.onClick.RemoveListener(OnClickExit);            
        }

        /// <summary>
        /// 设置文本内容
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        public void SetText(string title, string content)
        {
            textTitle.text = title;
            textOutput.text = content;
        }

        private void OnClickContinue()
        {
            onContinueSelected?.Invoke();
            Destroy();
        }

        private void OnClickRestart()
        {
            onRestartSelected?.Invoke();
            Destroy();
        }

        private void OnClickExit()
        {
            onExitSelected?.Invoke();
            Destroy();
        }
    }
}
