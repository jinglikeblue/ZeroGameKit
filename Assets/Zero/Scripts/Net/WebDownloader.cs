using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Zero
{
    /// <summary>
    /// 网络资源下载器
    /// </summary>
    public class WebDownloader: BaseWebDownloader
    {
        /// <summary>
        /// 下载的网络地址
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// 存放的本地地址
        /// </summary>
        public string savePath { get; private set; }

        /// <summary>
        /// 是否是超时
        /// </summary>
        public bool IsTimeout { get; private set; } = false;

        /// <summary>
        /// 下载超时的设置，当指定毫秒内下载进度没有改变时，视为下载超时。
        /// </summary>
        public int timeout = 15000;

        Coroutine _coroutine;

        /// <summary>
        /// 初始化下载类
        /// </summary>
        /// <param name="url">下载文件的URL地址</param>
        /// <param name="savePath">保存文件的本地地址</param>
        /// <param name="version">URL对应文件的版本号</param>
        public WebDownloader(string url, string savePath, string version = null)
        {
            Url = url;
            this.savePath = savePath;
            string saveDir = Path.GetDirectoryName(savePath);
            if (Directory.Exists(saveDir) == false)
            {
                Directory.CreateDirectory(saveDir);
            }

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

            _coroutine = ILBridge.Ins.StartCoroutine(this, DownloadProcess());
        }

        IEnumerator DownloadProcess()
        {
            var startTime = DateTime.Now;

            var request = UnityWebRequest.Get(Url);            
            var handler = new DownloadHandlerFile(savePath);            
            handler.removeFileOnAbort = true;
            request.downloadHandler = handler;
            request.SendWebRequest();            
            
            while (false == request.isDone)
            {
                if (isCanceled)
                {
                    yield break;
                }

                var tn = DateTime.Now - startTime;
                if(tn.TotalMilliseconds > timeout)
                {
                    IsTimeout = true;
                    yield break;
                }

                if (0 == totalSize)
                {
                    var contentLengthStr = request.GetResponseHeader("Content-Length");
                    if (null != contentLengthStr)
                    {
                        totalSize = long.Parse(contentLengthStr);
                    }
                }                

                downloadedSize = (long)request.downloadedBytes;
                
                yield return 0;
            }

            if(request.error != null)
            {
                error = request.error;
            }
            else if (IsTimeout)
            {
                error = "Timeout";
            }

            isDone = true;
            
            request.Dispose();
            request.downloadHandler.Dispose();

            _coroutine = null;
        }

        public void Dispose()
        {
            if(false == isDone)
            {
                isCanceled = true;
            }

            if(null != _coroutine)
            {
                ILBridge.Ins.StopCoroutine(_coroutine);
            }
        }
    }
}
