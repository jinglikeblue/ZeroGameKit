using System;
using System.Collections;
using System.Collections.Generic;

namespace Zero
{
    /// <summary>
    /// 资源组顺序下载器
    /// </summary>
    public class GroupWebDownloader : BaseWebDownloader
    {
        /// <summary>
        /// 加载信息
        /// </summary>
        public struct LoadInfo
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
            public long fileSize;

            public LoadInfo(string url, string savePath, string version, long fileSize, Action<object> onLoaded, object data)
            {
                this.url = url;
                this.savePath = savePath;
                this.version = version;
                this.fileSize = fileSize;
                this.onLoaded = onLoaded;
                this.data = data;
            }
        }

        public int Count
        {
            get { return _infoList.Count; }
        }

        List<LoadInfo> _infoList = new List<LoadInfo>();
        int _idx;
        bool _isLoadding = false;

        public void AddLoad(string url, string savePath, string version, long fileSize = 1, Action<object> onLoaded = null, object data = null)
        {
            if (_isLoadding)
            {
                return;
            }
            _infoList.Add(new LoadInfo(url, savePath, version, fileSize, onLoaded, data));
            totalSize += fileSize;
        }

        public void StartLoad()
        {
            if (_isLoadding)
            {
                return;
            }
            downloadedSize = 0;

            ILBridge.Ins.StartCoroutine(this, DownloadProcess());            
        }

        IEnumerator DownloadProcess()
        {
            _isLoadding = true;
            
            _idx = 0;

            while (_idx < _infoList.Count)
            {
                LoadInfo info = _infoList[_idx];
                WebDownloader loader = new WebDownloader(info.url, info.savePath, info.version);
                do
                {
                    double loaderLoaded = info.fileSize * loader.progress;
                    var tempLoadedSize = downloadedSize + loaderLoaded;
                    //Debug.LogFormat("下载进度  idx:{0} , progress:{1}[{2}/{3}]", _idx, _progress, tempLoadedSize, _totalSize);

                    yield return 0;
                }
                while (false == loader.isDone);

                if (loader.error != null)
                {
                    error = string.Format("[{0}] {1}", info.url, loader.error);
                    break;
                }

                info.onLoaded?.Invoke(info.data);

                downloadedSize += info.fileSize;
                _idx++;
            }

            downloadedSize = totalSize;
            isDone = true;
            _isLoadding = false;
        }
    }
}
