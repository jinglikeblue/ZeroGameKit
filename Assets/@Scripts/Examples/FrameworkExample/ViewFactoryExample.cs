using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using Zero;

namespace Example
{
    class ViewFactoryExample
    {
        static public void Start()
        {
            UIWinMgr.Ins.Open<ViewFactoryExampleWin>();
        }
    }

    class ViewFactoryExampleWin : WithCloseButtonWin
    {
        public Button btnShowClock;

        ClockView _clock;

        int i = 0;

        protected override void OnInit(object data)
        {
            base.OnInit(data);            
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            btnShowClock.onClick.AddListener(ShowClock);            
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            btnShowClock.onClick.RemoveListener(ShowClock);
        }

        private void ShowClock()
        {
            if(null != _clock)
            {
                //销毁旧的
                _clock.Destroy();
            }

            var content = GetChild("ClockShowContent");


            i++;
            if(i %2 == 1)
            {
                //同步方式之一
                var prefab = ResMgr.Load<GameObject>(AB.EXAMPLES_FRAMEWORK.ClockView_assetPath);
                _clock = ViewFactory.Create<ClockView>(prefab, content, DateTime.Now);
                Debug.Log(LogColor.Zero1($"同步视图创建完成:{_clock.GetType()}"));
            }
            else
            {
                //异步方式之一
                ViewFactory.CreateAsync<ClockView>(AB.EXAMPLES_FRAMEWORK.NAME, AB.EXAMPLES_FRAMEWORK.ClockView, content, DateTime.Now, 
                (ClockView view)=>
                {
                    Debug.Log(LogColor.Zero2($"异步视图创建完成:{_clock.GetType()}"));
                    _clock = view;
                }, 
                (float progress)=>
                {
                    Debug.Log(LogColor.Zero2($"异步加载进度:{progress}"));
                }, 
                (UnityEngine.Object obj)=>
                {
                    Debug.Log(LogColor.Zero2($"异步加载完成 name:{obj.name} type:{obj.GetType()}"));
                });
            }

            //还有很多方式，可以自行尝试
            
        }

        class ClockView : AView
        {
            public Text text;

            protected override void OnInit(object data)
            {
                base.OnInit(data);
                DateTime dt = (DateTime)data;
                text.text = dt.ToString();
            }
        }
    }
}
