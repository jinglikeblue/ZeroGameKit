using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Zero
{
    /// <summary>
    /// Http下载工具
    /// </summary>
    public class HttpDownloader
    {
        //下载进度更新事件
        public delegate void ProgressEvent(HttpDownloader downloader, float progress, int contentLength);

        //下载结束事件
        public delegate void CompletedEvent(HttpDownloader downloader);

        //收到请求返回的Headers数据
        public delegate void ReceivedResponseHeadersEvent(HttpDownloader downloader, Dictionary<string, string> responseHeaders);

        HttpDownloadHandler _handler;
        public UnityWebRequest request { get; private set; }

        /// <summary>
        /// 事件：收到请求返回的协议头数据
        /// </summary>
        public event ReceivedResponseHeadersEvent onResponseHeaders;

        /// <summary>
        /// 事件：下载进度更新
        /// </summary>
        public event ProgressEvent onProgress;

        /// <summary>
        /// 事件：下载完成
        /// </summary>
        public event CompletedEvent onCompleted;

        public bool isDone
        {
            get
            {
                return request.isDone;
            }
        }

        public float progress
        {
            get
            {
                return request.downloadProgress;
            }
        }

        public string error
        {
            get
            {
                return request.error;
            }
        }

        public long totalSize
        {
            get
            {
                return _handler.totalSize;
            }
        }


        /// <summary>
        /// 已完成大小
        /// </summary>
        public long loadedSize
        {
            get
            {
                return _handler.downloadedSize;
            }
        }

        /// <summary>
        /// 是否已销毁
        /// </summary>
        public bool isDisposed { get; private set; } = false;

        /// <summary>
        /// 下载的URL地址
        /// </summary>
        public string url { get; private set; }


        /// <summary>
        /// 文件的保存路径
        /// </summary>
        public string savePath { get; private set; }

        /// <summary>
        /// 请求超时秒数
        /// </summary>
        public int timeout = 0;

        UnityWebRequestAsyncOperation _asyncOperation;

        /// <summary>
        /// 初始化下载类
        /// </summary>
        /// <param name="url">下载文件的URL地址</param>
        /// <param name="savePath">保存文件的本地地址</param>
        /// <param name="version">URL对应文件的版本号</param>
        /// <param name="isResumeable">是否开启断点续传</param>
        public HttpDownloader(string url, string savePath, string version = null, bool isResumeable = false)
        {
            this.url = url;
            this.savePath = savePath;

            if (null != version)
            {
                string flag = url.Contains("?")?"&":"?";
                url += string.Format("{0}unity_download_ver={1}", flag, version);
            }

            //Debug.Log($"下载文件:{url}  保存位置:{savePath}  版本号:{version} 是否断点续传:{isResumeable}");
                   
            request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
            _handler = new HttpDownloadHandler(savePath, isResumeable, request);
            request.downloadHandler = _handler;
            if (isResumeable)
            {                           
                if (_handler.downloadedSize > 0)
                {
                    Debug.Log($"[HttpDownloader] 断点续传文件[{Path.GetFileName(savePath)}] 从字节数[{_handler.downloadedSize}]开始");
                    //断点续传的头数据
                    request.SetRequestHeader("Range", "bytes=" + _handler.downloadedSize + "-");
                }
            }         
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        public void Start()
        {
            if(null != _asyncOperation)
            {
                Debug.LogWarning("HttpDownloader的Start不能重复调用");
                return;
            }

            _handler.onReceivedHeaders += OnReceivedHeaders;
            _handler.onReceivedData += OnHandlerReceivedData;
            request.timeout = timeout;            
            _asyncOperation = request.SendWebRequest();            
            _asyncOperation.completed += OnRequestCompleted;
        }

        private void OnReceivedHeaders()
        {
            var headers = request.GetResponseHeaders();
            onResponseHeaders?.Invoke(this, headers);
        }

        private void OnHandlerReceivedData(int contentLength)
        {
            onProgress?.Invoke(this, _handler.progress, contentLength);
        }

        private void OnRequestCompleted(AsyncOperation ao)
        {
            onCompleted?.Invoke(this);
        }

        public void StopAndDispose(bool isCleanTmepFile = false)
        {
            if(null == _asyncOperation)
            {
                Debug.LogWarning("HttpDownloader并未Start");
                return;
            }

            if (isDisposed)
            {
                Debug.LogWarning("HttpDownloader的Stop不能重复调用");
                return;
            }

            _handler.onReceivedHeaders -= OnReceivedHeaders;
            _handler.onReceivedData -= OnHandlerReceivedData;
            _asyncOperation.completed -= OnRequestCompleted;

            request?.Abort();
            _handler.DisposeSafely(isCleanTmepFile);
            isDisposed = true;
        }
    }
}
