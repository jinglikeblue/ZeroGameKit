using Jing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using ZeroHot;

namespace Example
{
    /// <summary>
    /// StreamingAssets中直接加载热更资源的功能研发
    /// </summary>
    class HotFilesLoadFuture
    {
        static public void Start()
        {
            UIWinMgr.Ins.Open<HotFilesLoadFutureWin>();
        }
    }

    class HotFilesLoadFutureWin : WithCloseButtonWin
    {        
        public Button btnLoadResJson;
        public Button btnLoadSettingJson;        
        public Button btnLoadDll;
        public Button btnLoadPrivacyPolicy;

        public Text textLog;

        public void L(string v)
        {
            textLog.text += "\r\n" + v;
        }


        protected override void OnEnable()
        {
            base.OnEnable();

            btnLoadResJson.onClick.AddListener(LoadResJson);
            btnLoadSettingJson.onClick.AddListener(LoadSettingJson);            
            btnLoadDll.onClick.AddListener(LoadDll);
            btnLoadPrivacyPolicy.onClick.AddListener(LoadPrivacyPolicy);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            btnLoadResJson.onClick.RemoveListener(LoadResJson);
            btnLoadSettingJson.onClick.RemoveListener(LoadSettingJson);            
            btnLoadDll.onClick.RemoveListener(LoadDll);
            btnLoadPrivacyPolicy.onClick.RemoveListener(LoadPrivacyPolicy);
        }

        private void LoadResJson()
        {
            L("加载 res.json");            
            this.StartCoroutine(LoadText("res.json"));
        }

        private void LoadSettingJson()
        {
            L("加载 setting.json");            
            this.StartCoroutine(LoadText("setting.json"));
        }

        private void LoadDll()
        {
            L("加载 dll");            
            this.StartCoroutine(LoadBytes("dll/scripts.dll"));            
            this.StartCoroutine(LoadBytes("dll/scripts.pdb"));
        }

        private void LoadPrivacyPolicy()
        {
            L("加载 configs");
            this.StartCoroutine(LoadText(HotFiles._隐私政策_TXT));                               
        }

        IEnumerator LoadText(string path)
        {
            L($"LoadText: {path}");
            var loader = HotFilesMgr.Ins.LoadText(path);
            while (!loader.isDone)
            {
                yield return 0;
            }

            if (loader.error != null)
            {
                L(loader.error);
                yield break;
            }

            Debug.Log(loader.text);
            L($"加载内容长度: {loader.text.Length}");
            L("------------------------------------------------------------");
        }

        IEnumerator LoadBytes(string path)
        {
            L($"LoadBytes: {path}");
            var loader = HotFilesMgr.Ins.LoadBytes(path);
            while (!loader.isDone)
            {
                yield return 0;
            }

            if (loader.error != null)
            {
                L(loader.error);
                yield break;
            }
            
            L($"加载内容长度: {loader.bytes.Length}");
            L("------------------------------------------------------------");
        }
    }
}
