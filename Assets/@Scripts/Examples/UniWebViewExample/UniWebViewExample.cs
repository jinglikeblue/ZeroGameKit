using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using ZeroGameKit;

namespace Example
{
    class UniWebViewExample
    {
        static public void Start()
        {
            UIWinMgr.Ins.Open<UniWebViewExampleWin>();
        }
    }

    class UniWebViewExampleWin : WithCloseButtonWin
    {
        public GameObject textTip;
        public GameObject content;

        protected override void OnInit(object data)
        {
            base.effectEnable = false;
            base.OnInit(data);            
        }        

        protected override void OnEnable()
        {
            base.OnEnable();

            switch (Application.platform)
            {
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.Android:
                    textTip.SetActive(false);
                    content.SetActive(true);
                    break;
                default:
                    textTip.SetActive(true);
                    content.SetActive(false);
                    break;

            }
        }
    }
}
