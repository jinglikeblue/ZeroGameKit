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
            /// 加载完成的回调
            /// </summary>
            public Action<object> onLoaded;

            /// <summary>
            /// 加载完成回调携带的数据
            /// </summary>
            public object data;

            /// <summary>
            /// 加载文件的大小(bytes)
            /// </summary>
            public long fileVirtualSize;

            public TaskInfo(string url, string savePath, string version, long fileVirtualSize, Action<object> onLoaded, object data)
            {
                this.url = url;
                this.savePath = savePath;
                this.version = version;
                this.fileVirtualSize = fileVirtualSize;
                this.onLoaded = onLoaded;
                this.data = data;
            }
        }
        #endregion

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
        int _currentTaskIndex = -1;

        /// <summary>
        /// 当前下载任务的信息
        /// </summary>
        TaskInfo _currentTaskInfo;

        /// <summary>
        /// 当前下载任务的下载器
        /// </summary>
        HttpDownloader _currentDownloader;

        /// <summary>
        /// 标记是否正在下载中
        /// </summary>
        public bool IsStarted
        {
            get
            {
                return _currentTaskIndex > -1 ? true : false;
            }
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
        /// <param name="onLoaded">任务完成后的回调</param>
        /// <param name="data">任务完成后，回调是返回的数据</param>
        public void AddTask(string url, string savePath, string version, long fileSize = 1, Action<object> onLoaded = null, object data = null)
        {
            if (IsStarted)
            {
                Debug.LogWarning($"GroupHttpDownloader执行Start后无法再AddTask");
                return;
            }

            _infoList.Add(new TaskInfo(url, savePath, version, fileSize, onLoaded, data));
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
            _currentTaskIndex++;
            if(_currentTaskIndex < _infoList.Count)
            {
                //继续下载                
                _currentTaskInfo = _infoList[_currentTaskIndex];
                _currentDownloader = new HttpDownloader(_currentTaskInfo.url, _currentTaskInfo.savePath, _currentTaskInfo.version, _isResumeable);                
                _currentDownloader.onProgress += OnHttpDownloaderProgress;
                _currentDownloader.onCompleted += OnHttpDownloaderCompleted;
                _currentDownloader.Start();
            }
            else
            {                
                //下载完成
                End();
            }
        }

        private void OnHttpDownloaderCompleted(HttpDownloader downloader)
        {
            if(downloader.error != null)
            {
                //下载出错了
                var error = $"[{downloader.url}] {downloader.error}";
                End(error);
            }

            _loadCompletedFileSize += _currentTaskInfo.fileVirtualSize;
            loadedSize = _loadCompletedFileSize;

            _currentTaskInfo.onLoaded?.Invoke(_currentTaskInfo.data);
            DownloadNext();
        }

        private void OnHttpDownloaderProgress(HttpDownloader downloader, float progress, int contentLength)
        {
            var virtualSize = progress * _currentTaskInfo.fileVirtualSize;

            loadedSize = _loadCompletedFileSize + (long)virtualSize;
            this.progress = (float)Math.Round((double)loadedSize / totalSize, 4);

            Debug.Log($"[GroupHttpDownloader] progress:{this.progress} loaded:{loadedSize}/{totalSize}");
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
            _currentTaskInfo = null;
            if (_currentDownloader != null)
            {
                _currentDownloader.onProgress -= OnHttpDownloaderProgress;
                _currentDownloader.onCompleted -= OnHttpDownloaderCompleted;
                _currentDownloader.StopAndDispose(isCleanTmepFile);
                _currentDownloader = null;
            }            
            _currentTaskIndex = int.MaxValue;
            this.error = error;            
            isDone = true;            
        }
    }
}
