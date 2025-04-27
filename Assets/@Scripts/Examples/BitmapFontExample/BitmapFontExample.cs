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
    class BitmapFontExample
    {
        public static void Start()
        {
            UIWinMgr.Ins.Open<BitmapFontExampleWin>();
        }
    }


    class BitmapFontExampleWin: WithCloseButtonWin
    {        
        Text textDynamic;

        float _time = 0;

        protected override void OnDisable()
        {
            base.OnDisable();
            ILBridge.Ins.onUpdate -= OnUpdate;            
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            ILBridge.Ins.onUpdate += OnUpdate;            
        }

        private void OnUpdate()
        {
            _time += Time.deltaTime;
            textDynamic.text = ((int)_time).ToString();
        }
    }
}
