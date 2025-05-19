using Jing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;
using Zero;

namespace Example
{
    /// <summary>
    /// 读取@Files中的文件
    /// </summary>
    class HotFilesExample
    {
        static public void Start()
        {
            UIWinMgr.Ins.Open<HotFilesExampleWin>();
        }
    }

    class HotFilesExampleWin : WithCloseButtonWin
    {        
        public Button btnAllFiles;
        public Button btnPicsFiles;        
        public Button btnLoadSampleMP4;
        public Button btnLoadPrivacyPolicy;

        public Text textLog;

        public void L(string v)
        {
            textLog.text += "\r\n" + v;
        }


        protected override void OnEnable()
        {
            base.OnEnable();

            btnAllFiles.onClick.AddListener(AllFiles);
            btnPicsFiles.onClick.AddListener(PicsFiles);
            btnLoadSampleMP4.onClick.AddListener(LoadSampleMP4);
            btnLoadPrivacyPolicy.onClick.AddListener(LoadPrivacyPolicy);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            btnAllFiles.onClick.RemoveListener(AllFiles);
            btnPicsFiles.onClick.RemoveListener(PicsFiles);
            btnLoadSampleMP4.onClick.RemoveListener(LoadSampleMP4);
            btnLoadPrivacyPolicy.onClick.RemoveListener(LoadPrivacyPolicy);
        }

        private void AllFiles()
        {
            var list = R.AllFiles();
            var json = Json.ToJsonIndented(list);
            L($"所有的文件：{json}");            
            L("------------------------------------------------------------");
        }

        private void PicsFiles()
        {
            var list0 = R.Find("pics");
            // var list1 = R.Find("pics", System.IO.SearchOption.AllDirectories);

            L($"pics下的文件：{Json.ToJsonIndented(list0)}");

            // L($"pics下的文件(含子目录)：{Json.ToJsonIndented(list1)}");

            L("------------------------------------------------------------");
        }

        private void LoadSampleMP4()
        {
            L($"加载 {R.Sample_mp4}");
            this.StartCoroutine(LoadBytes(R.Sample_mp4));
        }

        private async void LoadPrivacyPolicy()
        {
            L($"加载 {R._Files_privacy_policy_txt}");
            L($"FullPath: {Res.AbsolutePath(R._Files_privacy_policy_txt)}");
            var text = await Res.LoadAsync<string>(R._Files_privacy_policy_txt);
            if (null == text)
            {
                L($"加载内容不存在: {R._Files_privacy_policy_txt}");
            }
            
            Debug.Log(text);
            L($"加载内容长度: {text.Length}");
            L("------------------------------------------------------------");
        }

        IEnumerator LoadBytes(string path)
        {
            L($"FullPath: {Res.AbsolutePath(path)}");
            var loader = Res.LoadAsync<byte[]>(path);
            
            while (loader.Status == UniTaskStatus.Pending)
            {
                yield return 0;
            }

            if (loader.Status != UniTaskStatus.Succeeded)
            {
                L(loader.Status.ToString());
                yield break;
            }

            var bytes = loader.GetAwaiter().GetResult();
            L($"加载内容长度: {bytes.Length}");
            L("------------------------------------------------------------");
        }
    }
}
