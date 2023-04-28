using Jing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    class ZipExample
    {
        static public void Start()
        {
            UIWinMgr.Ins.Open<ZipExampleWin>();
        }
    }

    class ZipExampleWin : WithCloseButtonWin
    {
        public Button btnCompress;
        public Button btnUncompress;
        public Button btnCancelCompress;


        public Text textLog;

        public void L(string v)
        {
            textLog.text += "\r\n" + v;
        }

        string _zipFileName;
        string _fileDir;

        string _rootDir;

        protected override void OnInit(object data)
        {
            base.OnInit(data);

            //创建临时文件，用来测试压缩            
            _rootDir = FileUtility.CombineDirs(true, Application.temporaryCachePath, "test_zip");

            //要压缩的目录
            _fileDir = FileUtility.CombineDirs(true, _rootDir, "files");
            Directory.CreateDirectory(_fileDir);

            for (var i = 0; i < 10; i++)
            {
                var tempFileName = FileUtility.CombinePaths(_fileDir, $"test_{i}.txt");
                File.WriteAllText(tempFileName, "hello world");
            }

            //压缩文件
            _zipFileName = FileUtility.CombinePaths(_rootDir, "file.zip");

            L($"测试文件目录:{_rootDir}");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            //销毁临时文件            
            if (Directory.Exists(_rootDir))
            {
                Directory.Delete(_rootDir, true);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            btnCompress.onClick.AddListener(Compress);
            btnUncompress.onClick.AddListener(Uncompress);
            btnCancelCompress.onClick.AddListener(CancelCompress);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            btnCompress.onClick.RemoveListener(Compress);
            btnUncompress.onClick.RemoveListener(Uncompress);
            btnCancelCompress.onClick.RemoveListener(CancelCompress);
        }

        ZipHelper _compressHelper;

        private void Compress()
        {
            StartCoroutine(CompressCoroutine());
        }

        IEnumerator CompressCoroutine()
        {
            var compressHelper = new ZipHelper();
            _compressHelper = compressHelper;
            _compressHelper.CompressAsync(_zipFileName, _fileDir);
            while (null != _compressHelper && false == _compressHelper.isDone)
            {                
                L($"压缩进度:{_compressHelper.progressInfo}");
                yield return new WaitForSeconds(1f);
            }
            L($"压缩进度:{_compressHelper.progressInfo}");

            L($"压缩结束 error:{(compressHelper.error == null?"none":compressHelper.error)}");
        }

        private void CancelCompress()
        {
            if (_compressHelper != null)
            {
                _compressHelper.Cancel();
                _compressHelper = null;

                L($"取消压缩");
            }            
        }

        private void Uncompress()
        {
            var helper = new ZipHelper();
            helper.Uncompress(_zipFileName, _fileDir);
            L($"解压结束");
        }


    }
}
