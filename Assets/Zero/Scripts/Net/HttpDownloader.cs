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
        HttpDownloadHandler _handler;
        public UnityWebRequest request { get; private set; }

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
                return _handler.totalFileSize;
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
        /// TODO 应该叫isDisposed
        /// </summary>
        public bool isDisposeed { get; private set; } = false;

        /// <summary>
        /// 下载的URL地址
        /// </summary>
        public string url { get; private set; }


        /// <summary>
        /// 文件的保存路径
        /// </summary>
        public string savePath { get; private set; }


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
                string flag;
                if (url.Contains("?"))
                {
                    flag = "&";
                }
                else
                {
                    flag = "?";
                }

                url += string.Format("{0}unity_download_ver={1}", flag, version);
            }

            Debug.Log($"下载文件:{url}  保存位置:{savePath}  版本号:{version} 是否断点续传:{isResumeable}");

            _handler = new HttpDownloadHandler(savePath, isResumeable);
            request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET, _handler, null);
            if (isResumeable)
            {
                //断点续传的头数据
                request.SetRequestHeader("Range", "bytes=" + _handler.downloadedSize + "-");
            }
            request.SendWebRequest();
        }

        

        public void Dispose(bool isCleanTmepFile = false)
        {
            if (false == isDisposeed)
            {
                Debug.Log($"Handler is Done: {_handler.isDone}");
                _handler.DisposeSafely(isCleanTmepFile);
                isDisposeed = true;
            }
            //_handler = null;
        }
    }
}
