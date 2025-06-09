using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 资源下载队列
    /// </summary>
    public class GroupHttpDownloader
    {
        #region 委托定义
        //下载进度更新事件
        public delegate void ProgressEvent(GroupHttpDownloader groupDownloader, float progress, int contentLength);

        //单个任务开始的事件
        public delegate void TaskStartedEvent(GroupHttpDownloader groupDownloader, TaskInfo taskInfo, Dictionary<string, string> responseHeaders);

        //单个任务完成的事件
        public delegate void TaskCompletedEvent(GroupHttpDownloader groupDownloader, TaskInfo taskInfo);

        //队列完成的事件
        public delegate void CompletedEvent(GroupHttpDownloader groupDownloader);
        #endregion

        #region 下载内容信息结构体
        /// <summary>
        /// 加载信息
        /// </summary>
        public class TaskInfo
        {
            /// <summary>
            /// 加载对象URL
            /// </summary>
            public string url;

            /// <summary>
            /// 保存位置
            /// </summary>
            public string savePath;

            /// <summary>
            /// 文件版本号
            /// </summary>
            public string version;

            /// <summary>
            /// 附加的数据
            /// </summary>
            public object data;

            /// <summary>
            /// 加载文件的大小(bytes)
            /// </summary>
            public long fileVirtualSize;

            public TaskInfo(string url, string savePath, string version, long fileVirtualSize, object data)
            {
                this.url = url;
                this.savePath = savePath;
                this.version = version;
                this.fileVirtualSize = fileVirtualSize;                
                this.data = data;
            }
        }
        #endregion

        /// <summary>
        /// 下载进度更新事件
        /// </summary>
        public event ProgressEvent onProgress;

        /// <summary>
        /// 单个任务开始的事件
        /// </summary>
        public event TaskStartedEvent onTaskStarted;

        /// <summary>
        /// 单个任务完成的事件
        /// </summary>
        public event TaskCompletedEvent onTaskCompleted;

        /// <summary>
        /// 队列完成的事件
        /// </summary>
        public event CompletedEvent onCompleted;

        /// <summary>
        /// 已下载的文件大小
        /// </summary>
        public long loadedSize { get; private set; } = 0;

        /// <summary>
        /// 下载文件总大小
        /// </summary>
        public long totalSize { get; private set; } = 0;

        /// <summary>
        /// 下载进度
        /// </summary>
        public float progress { get; private set; } = 0;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string error { get; private set; } = null;

        /// <summary>
        /// 下载是否完成
        /// </summary>
        public bool isDone { get; private set; } = false;

        /// <summary>
        /// 下载文件总数
        /// </summary>
        public int Count
        {
            get { return _infoList.Count; }
        }

        /// <summary>
        /// 下载文件信息队列
        /// </summary>
        List<TaskInfo> _infoList = new List<TaskInfo>();

        /// <summary>
        /// 当前下载任务的序号
        /// </summary>
        public int currentTaskIndex { get; private set; } = -1;

        /// <summary>
        /// 当前下载任务的信息
        /// </summary>
        public TaskInfo currentTaskInfo { get; private set; } = null;

        /// <summary>
        /// 当前下载任务的下载器
        /// </summary>
        public HttpDownloader currentDownloader { get; private set; } = null;

        /// <summary>
        /// 请求超时秒数
        /// </summary>
        public int timeout = 60;

        /// <summary>
        /// 标记是否正在下载中
        /// </summary>
        public bool IsStarted
        {
            get { return currentTaskIndex > -1 ? true : false; }
        }

        /// <summary>
        /// 下载完成的文件的总大小
        /// </summary>
        long _loadCompletedFileSize = 0;

        bool _isResumeable = false;        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isResumeable">是否启动断点续传</param>
        public GroupHttpDownloader(bool isResumeable = false)
        {
            _isResumeable = isResumeable;
        }

        /// <summary>
        /// 添加下载任务
        /// </summary>
        /// <param name="url">下载文件地址</param>
        /// <param name="savePath">保存文件路径</param>
        /// <param name="version">版本号</param>
        /// <param name="fileSize">文件大小（虚拟值，用来计算列表下载进度）</param>        
        /// <param name="data">任务完成后，回调是返回的数据</param>
        public void AddTask(string url, string savePath, string version, long fileSize = 1, object data = null)
        {
            if (IsStarted)
            {
                Debug.LogWarning($"GroupHttpDownloader执行Start后无法再AddTask");
                return;
            }

            _infoList.Add(new TaskInfo(url, savePath, version, fileSize, data));
            totalSize += fileSize;
        }

        public void Start()
        {
            if (IsStarted)
            {
                Debug.LogWarning($"GroupHttpDownloader的Start无法重复执行");
                return;
            }

            if (isDone)
            {
                Debug.LogWarning($"Start在isDone为true后无法调用");
                return;
            }

            DownloadNext();
        }

        void DownloadNext()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                End("Editor运行状态已退出!", true);
                return;
            }
#endif
            currentTaskIndex++;
            if(currentTaskIndex < _infoList.Count)
            {
                //继续下载                
                currentTaskInfo = _infoList[currentTaskIndex];
                currentDownloader = new HttpDownloader(currentTaskInfo.url, currentTaskInfo.savePath, currentTaskInfo.version, _isResumeable);
                currentDownloader.timeout = timeout;
                currentDownloader.onProgress += OnHttpDownloaderProgress;
                currentDownloader.onCompleted += OnHttpDownloaderCompleted;
                currentDownloader.onResponseHeaders += OnHttpDownloaderResponseHeaders;
                currentDownloader.Start();
            }
            else
            {                
                //下载完成
                End();
            }
        }

        private void OnHttpDownloaderResponseHeaders(HttpDownloader downloader, Dictionary<string, string> responseHeaders)
        {
            onTaskStarted?.Invoke(this, currentTaskInfo, responseHeaders);
        }

        private void OnHttpDownloaderCompleted(HttpDownloader downloader)
        {
            if(downloader.error != null)
            {
                //下载出错了
                var error = $"[{downloader.url}] {downloader.error}";
                End(error);
            }

            _loadCompletedFileSize += currentTaskInfo.fileVirtualSize;
            loadedSize = _loadCompletedFileSize;            

            onTaskCompleted?.Invoke(this, currentTaskInfo);

            DownloadNext();
        }

        private void OnHttpDownloaderProgress(HttpDownloader downloader, float progress, int contentLength)
        {
            var virtualSize = progress * currentTaskInfo.fileVirtualSize;

            loadedSize = _loadCompletedFileSize + (long)virtualSize;
            this.progress = (float)Math.Round((double)loadedSize / totalSize, 4);

            //Debug.Log($"[GroupHttpDownloader] progress:{this.progress} loaded:{loadedSize}/{totalSize}");

            onProgress?.Invoke(this, this.progress, contentLength);
        }

        public void StopAndDispose(bool isCleanTmepFile = false)
        {
            if(false == IsStarted)
            {
                Debug.LogWarning($"StopAndDispose在没有Start时无法调用");
                return;
            }

            if (isDone)
            {
                Debug.LogWarning($"StopAndDispose在isDone为true后无法调用");
                return;
            }

            End("Stopped", isCleanTmepFile);
        }

        void End(string error = null, bool isCleanTmepFile = false)
        {
            currentTaskInfo = null;
            if (currentDownloader != null)
            {
                currentDownloader.onProgress -= OnHttpDownloaderProgress;
                currentDownloader.onCompleted -= OnHttpDownloaderCompleted;
                currentDownloader.onResponseHeaders -= OnHttpDownloaderResponseHeaders;
                currentDownloader.StopAndDispose(isCleanTmepFile);
                currentDownloader = null;
            }            
            currentTaskIndex = int.MaxValue;
            this.error = error;            
            isDone = true;
            onCompleted?.Invoke(this);
            if (!string.IsNullOrEmpty(error))
            {
                Debug.Log($"[GroupHttpDownloader] 下载结束:  {error}");
            }
        }
    }
}
