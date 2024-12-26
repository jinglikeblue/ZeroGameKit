using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using ZeroGameKit;

namespace Example
{
    class UniTaskExample
    {
        static public void Start()
        {
            UIWinMgr.Ins.Open<UniTaskExampleWin>();
        }
    }

    class UniTaskExampleWin : WithCloseButtonWin
    {
        public Text textTip;

        private List<string> _contentLineList;

        protected override void OnEnable()
        {
            base.OnEnable();
            _contentLineList = new List<string>();
            using (StringReader reader = new StringReader(textTip.text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    _contentLineList.Add(line);
                }
            }
            
            textTip.text = string.Empty;
            StartLoopShow();
        }

        private async void StartLoopShow()
        {
            try
            {
                foreach (var line in _contentLineList)
                {
                    textTip.text += line + "\r\n";
                    await UniTask.Delay(1000);
                    if (gameObject == null)
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}