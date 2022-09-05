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
            var list = HotFiles.GetAllFileList();
            var json = LitJson.JsonMapper.ToPrettyJson(list);
            L($"所有的文件：{json}");            
            L("------------------------------------------------------------");
        }

        private void PicsFiles()
        {
            var list0 = HotFiles.GetFiles("pics");
            var list1 = HotFiles.GetFiles("pics", System.IO.SearchOption.AllDirectories);

            L($"pics下的文件(不含子目录)：{LitJson.JsonMapper.ToPrettyJson(list0)}");

            L($"pics下的文件(含子目录)：{LitJson.JsonMapper.ToPrettyJson(list1)}");

            L("------------------------------------------------------------");
        }

        private void LoadSampleMP4()
        {
            L($"加载 {HotFiles.VIDEOS_SAMPLE_MP4}");
            this.StartCoroutine(LoadBytes(HotFiles.VIDEOS_SAMPLE_MP4));
        }

        private void LoadPrivacyPolicy()
        {
            L($"加载 {HotFiles._隐私政策_TXT}");
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
