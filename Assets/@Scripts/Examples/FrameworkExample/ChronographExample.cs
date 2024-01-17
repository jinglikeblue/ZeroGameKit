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
    class ChronographExample
    {
        static public void Start()
        {
            UIWinMgr.Ins.Open<ChronographExampleWin>();
        }
    }

    public class ChronographExampleWin : WithCloseButtonWin
    {
        public Button btnStart;
        public Button btnPause;        
        public Button btnStop;
        public Text textTime;
        Chronograph _chronograph;


        protected override void OnInit(object data)
        {
            base.OnInit(data);

            _chronograph = new Chronograph();

            btnStart.onClick.AddListener(() => {
                _chronograph.Start();
            });

            btnPause.onClick.AddListener(() => {
                _chronograph.Pause();
            });

            btnStop.onClick.AddListener(() => {
                _chronograph.Stop();
            });
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            ILBridge.Ins.onUpdate += RefreshUI;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ILBridge.Ins.onUpdate -= RefreshUI;
        }

        void RefreshUI()
        {
            btnStart.interactable = false;
            btnPause.interactable = false;
            //btnStop.interactable = true;

            if (_chronograph.IsRunning)
            {
                btnPause.interactable = true;
                //btnStop.interactable = true;
            }
            else
            {
                btnStart.interactable = true;
            }

            textTime.text = _chronograph.ElapsedMilliseconds.ToString();
        }
    }
}
