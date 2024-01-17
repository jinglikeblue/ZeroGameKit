using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using ZeroHot;

namespace Example
{
    class TimerExample
    {
        static public void Start()
        {
            UIWinMgr.Ins.Open<TimerExampleWin>();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TimerExampleWin : WithCloseButtonWin
    {
        public Button btnStart;
        public Button btnPause;
        public Button btnContinue;
        public Button btnStop;
        public Text textLog;

        Timer _timer;

        protected override void OnInit(object data)
        {
            base.OnInit(data);

            _timer = new Timer(5);

            RefreshUI();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            _timer.onTriggered += OnTimerTriggered;
            _timer.onComplete += OnTimerComplete;

            btnStart.onClick.AddListener(OnBtnStartClick);
            btnPause.onClick.AddListener(OnBtnPauseClick);
            btnContinue.onClick.AddListener(OnBtnContinueClick);
            btnStop.onClick.AddListener(OnBtnStopClick);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _timer.onTriggered -= OnTimerTriggered;
            _timer.onComplete -= OnTimerComplete;

            btnStart.onClick.RemoveListener(OnBtnStartClick);
            btnPause.onClick.RemoveListener(OnBtnPauseClick);
            btnContinue.onClick.RemoveListener(OnBtnContinueClick);
            btnStop.onClick.RemoveListener(OnBtnStopClick);
        }

        private void OnTimerTriggered(Timer t)
        {
            L(Log.Zero1($"OnTimerTriggered：repeatCount={t.repeatCount} , triggeredTimes={t.triggeredTimes}, RemainingSeconds={t.RemainingSeconds}"));
            RefreshUI();
        }

        private void OnTimerComplete(Timer t)
        {
            L(Log.Zero2($"OnTimerComplete：repeatCount={t.repeatCount} , triggeredTimes={t.triggeredTimes}, RemainingSeconds={t.RemainingSeconds}"));
            RefreshUI();
        }

        private void OnBtnPauseClick()
        {
            _timer.Pause();
            RefreshUI();
            L($"暂停计时器  距离下次触发剩余的时间：{_timer.RemainingSeconds}");
        }

        private void OnBtnContinueClick()
        {
            _timer.Continue();
            RefreshUI();
            L($"继续计时器 距离下次触发剩余的时间：{_timer.RemainingSeconds}");
        }

        private void OnBtnStopClick()
        {
            _timer.Stop();
            RefreshUI();
            L($"停止计时器");
        }

        private void OnBtnStartClick()
        {
            _timer.Start(2);
            RefreshUI();
            L($"启动计时器 间隔{_timer.intervalSeconds}秒， 计时器将重复{_timer.repeatCount}次");
        }

        void RefreshUI()
        {
            btnStart.interactable = true;

            btnPause.interactable = false;
            btnContinue.interactable = false;
            btnStop.interactable = false;

            switch (_timer.state)
            {
                //case Timer.EState.IDLE:
                //    btnStart.enabled = true;
                //    break;
                case Timer.EState.RUNNING:
                    btnPause.interactable = true;
                    btnStop.interactable = true;
                    break;
                case Timer.EState.PAUSING:
                    btnContinue.interactable = true;
                    btnStop.interactable = true;
                    break;
            }
        }

        void L(string v)
        {
            textLog.text += "\r\n" + v;
        }
    }
}
