using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Zero
{
    /// <summary>
    /// TODO：考虑是否下载的文件保存为临时文件，后缀.temp下载完成后再改名，这样断点续传好处理
    /// </summary>    
    public class HttpDownloadHandler : DownloadHandlerScript
    {
        /// <summary>
        /// 保存路径
        /// </summary>
        public string savePath { get; private set; }

        /// <summary>
        /// 是否断点续传
        /// </summary>
        public bool isResumeable { get; private set; }

        /// <summary>
        /// 已下载文件大小
        /// </summary>
        public long downloadedSize { get; private set; } = 0;

        /// <summary>
        /// 文件大小
        /// </summary>
        public long totalFileSize { get; private set; } = 0;

        /// <summary>
        /// 收到数据的事件，参数是这次收到的数据长度
        /// </summary>
        public event Action<ulong> onReceivedData;

        FileStream _fileStream;
        HttpDownloader _downloader;

        string _tempSavePath;

        public HttpDownloadHandler(string savePath, bool isResumeable, HttpDownloader downloader)
        {            
            this.savePath = savePath;
            this.isResumeable = isResumeable;
            _downloader = downloader;
            _tempSavePath = savePath + ".temp";

            string saveDir = Path.GetDirectoryName(savePath);
            if (Directory.Exists(saveDir) == false)
            {
                Directory.CreateDirectory(saveDir);
            }

            FileMode mode = isResumeable ? FileMode.OpenOrCreate : FileMode.Create;

            _fileStream = new FileStream(_tempSavePath, mode, FileAccess.Write, FileShare.ReadWrite);

            if (isResumeable)
            {
                //断点续传的话，则先记录已下载的文件尺寸
                downloadedSize = _fileStream.Length;


                if(downloadedSize > 0)
                {
                    Debug.Log($"断点续传下载文件，已下载:{downloadedSize}");
                }                
            }
        }

        protected override void ReceiveContentLengthHeader(ulong contentLength)
        {
            var headers = _downloader.request.GetResponseHeaders();
            if (headers != null)
            {
                foreach (var kv in headers)
                {
                    Debug.Log($"Header: {kv.Key} = {kv.Value}");
                }
            }
            
            totalFileSize = (long)contentLength + downloadedSize;
            Debug.Log($"收到下载内容大小:{contentLength} 文件总大小:{totalFileSize}");            
        }

        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if(data == null || data.Length == 0)
            {
                return false;
            }

            if(_fileStream == null || _fileStream.CanWrite == false)
            {
                return false;
            }

            _fileStream.Write(data, 0, dataLength);
            downloadedSize += dataLength;

            Debug.Log($"下载到数据大小:{dataLength} 完成度:{GetProgress()} 已下载内容大小:{downloadedSize}/{totalFileSize}");


            return true;
        }

        protected override void CompleteContent()
        {                       
            Debug.Log($"下载完成:{savePath}");
            _fileStream.Flush();
            _fileStream.Close();
            _fileStream = null;            
        }

        protected override float GetProgress()
        {
            return base.GetProgress();
        }

        protected override byte[] GetData()
        {
            if(_fileStream != null)
            {
                return null;
            }
            return File.ReadAllBytes(savePath);            
        }

        protected override string GetText()
        {
            if (_fileStream != null)
            {
                return null;
            }
            return File.ReadAllText(savePath);
        }
    }
}
