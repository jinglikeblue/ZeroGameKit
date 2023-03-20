using Jing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zero;
using ZeroGameKit;

namespace Example
{
    /// <summary>
    /// UseCase:
    /// 暂停下载
    /// 继续下载
    /// 销毁下载
    /// 同时开启多个downloader去下载同一个文件
    /// </summary>
    class DownloadFileExample
    {
        public static void Start()
        {
            UIWinMgr.Ins.Open<DownloadFileExampleWin>();
        }
    }

    class DownloadFileExampleWin : WithCloseButtonWin
    {
        public InputField inputURL;
        public Button btnStart;
        public Button btnPause;
        public Toggle toggleResumeable;
        public Toggle toggleGroupDownload;
        public Text textLog;

        HttpDownloader _downloader;
        GroupHttpDownloader _groupDownloader;

        /// <summary>
        /// 是否使用协程方式加载的开关 true：使用协程方式下载  false：使用事件方式下载
        /// </summary>
        bool isUseCoroutineSwitch = false;

        void L(string content)
        {
            Debug.Log(content);
            textLog.text += $"{content}\r\n";
        }

        protected override void OnEnable()
        {
            btnStart.onClick.AddListener(StartDownload);
            btnPause.onClick.AddListener(StopDownload);
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            btnStart.onClick.RemoveListener(StartDownload);
            btnPause.onClick.RemoveListener(StopDownload);
            base.OnDisable();
        }

        private void StopDownload()
        {
            _downloader?.StopAndDispose();
        }

        private void StartDownload()
        {
            textLog.text = "";

            var url = inputURL.text;
            var fileName = Path.GetFileName(url);
            var savePath = FileUtility.CombinePaths(ZeroConst.PERSISTENT_DATA_PATH, fileName);
            L($"下载的文件: {url}");
            L($"保存的路径: {savePath}");

            string version = null;

            if (toggleGroupDownload.isOn)
            {
                TestGroupDownload(url, savePath, version);
            }
            else
            {
                TestDownload(url, savePath, version);
            }
        }

        #region GroupHttpDownloader

        void TestGroupDownload(string url, string savePath, string version)
        {
            _groupDownloader = new GroupHttpDownloader(toggleResumeable.isOn);
            _groupDownloader.onCompleted += OnGroupDownloaderCompleted;
            _groupDownloader.onProgress += OnGroupDownloaderProgress;
            _groupDownloader.onTaskCompleted += OnGroupDownloaderTaskCompleted;
            _groupDownloader.onTaskStarted += OnGroupDownloaderTaskStarted;
            _groupDownloader.AddTask(url, savePath, version, 10000);
            L($"添加队列下载任务 文件:{url}  保存位置:{savePath}  版本号:{version} 是否断点续传:{toggleResumeable.isOn}");
            _groupDownloader.AddTask(url, savePath, version, 10000);
            L($"添加队列下载任务 文件:{url}  保存位置:{savePath}  版本号:{version} 是否断点续传:{toggleResumeable.isOn}");
            _groupDownloader.Start();
        }

        private void OnGroupDownloaderCompleted(GroupHttpDownloader groupDownloader)
        {
            L($"下载完成 error:{groupDownloader.error}");
        }

        private void OnGroupDownloaderProgress(GroupHttpDownloader groupDownloader, float progress, int contentLength)
        {
            L($"下载到数据大小:{contentLength} 完成度:{progress} 已下载内容大小:{groupDownloader.loadedSize}/{groupDownloader.totalSize}");
        }

        private void OnGroupDownloaderTaskCompleted(GroupHttpDownloader groupDownloader, GroupHttpDownloader.TaskInfo taskInfo)
        {
            L($"下载任务完成 index:{groupDownloader.currentTaskIndex} url:{taskInfo.url}");
        }

        private void OnGroupDownloaderTaskStarted(GroupHttpDownloader groupDownloader, GroupHttpDownloader.TaskInfo taskInfo, Dictionary<string, string> responseHeaders)
        {
            L($"下载任务开始 index:{groupDownloader.currentTaskIndex} url:{taskInfo.url}");
        }

        #endregion

        #region HttpDownloader
        void TestDownload(string url, string savePath, string version)
        {
            _downloader = new HttpDownloader(url, savePath, version, toggleResumeable.isOn);

            L($"下载文件:{url}  保存位置:{savePath}  版本号:{version} 是否断点续传:{toggleResumeable.isOn}");

            if (toggleResumeable.isOn)
            {
                L($"断点续传下载文件，已下载:{_downloader.loadedSize}");
            }

            if (isUseCoroutineSwitch)
            {
                L($"使用协程方式进行下载：");
                StartCoroutine(DownlaodCoroutine());
            }
            else
            {
                L($"使用事件方式进行下载：");
                _downloader.onResponseHeaders += OnResponseHeaders;
                _downloader.onProgress += OnProgress;
                _downloader.onCompleted += OnCompleted;
                _downloader.Start();
            }

            //切换下载方式
            isUseCoroutineSwitch = !isUseCoroutineSwitch;
        }

        #region 协程下载
        IEnumerator DownlaodCoroutine()
        {
            _downloader.Start();
            while (false == _downloader.isDone)
            {
                L($"完成度:{_downloader.progress} 已下载内容大小:{_downloader.loadedSize}/{_downloader.totalSize}");
                yield return null;
            }
            L($"下载完成 error:{_downloader.error}");
        }
        #endregion

        #region 事件下载
        private void OnResponseHeaders(HttpDownloader downloader, Dictionary<string, string> responseHeaders)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"收到Response协议头:");
            foreach (var kv in responseHeaders)
            {
                sb.AppendLine($"{kv.Key} : {kv.Value}");
            }
            L(sb.ToString());

            L($"收到下载内容大小:{_downloader.totalSize - _downloader.loadedSize} 文件总大小:{_downloader.totalSize}");
        }

        private void OnProgress(HttpDownloader downloader, float progress, int contentLength)
        {
            L($"下载到数据大小:{contentLength} 完成度:{progress} 已下载内容大小:{_downloader.loadedSize}/{_downloader.totalSize}");
        }

        private void OnCompleted(HttpDownloader downloader)
        {
            L($"下载完成 error:{_downloader.error}");
        }
        #endregion

        #endregion
    }
}
