using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Zero
{
    /// <summary>
    /// 下载的文件保存为临时文件，后缀.temp。下载完成后再改名，这样断点续传好处理
    /// </summary>    
    public class HttpDownloadHandler : DownloadHandlerScript
    {
        public delegate void ReceivedData(int contentLength);

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
        public long totalSize { get; private set; } = 0;

        /// <summary>
        /// 下载进度
        /// </summary>
        public float progress { get; private set; } = 0;

        /// <summary>
        /// 收到数据的事件，参数是这次收到的数据长度
        /// </summary>
        //public event Action<int> onReceivedData;

        FileStream _fileStream;

        string _tempSavePath;

        /// <summary>
        /// 下载到了数据的更新
        /// </summary>
        public event ReceivedData onReceivedData;

        /// <summary>
        /// 收到了ResponseHeader数据
        /// </summary>
        public event Action onReceivedHeaders;

        UnityWebRequest _request;

        /// <summary>
        /// 对象是否已销毁
        /// </summary>
        public bool IsDisposed { get; private set; } = false;

        public HttpDownloadHandler(string savePath, bool isResumeable, UnityWebRequest request) : base(new byte[200 << 10])
        {
            _request = request;
            this.savePath = savePath;
            this.isResumeable = isResumeable;
            _tempSavePath = savePath + ".temp";

            string saveDir = Path.GetDirectoryName(savePath);
            if (Directory.Exists(saveDir) == false)
            {
                Directory.CreateDirectory(saveDir);
            }

            if (isResumeable)
            {
                downloadedSize = GetTempFileSize();
            }
        }

        /// <summary>
        /// 获得下载临时文件的大小
        /// </summary>
        /// <returns></returns>
        long GetTempFileSize()
        {
            if (false == File.Exists(_tempSavePath))
            {
                return 0;
            }

            return new FileInfo(_tempSavePath).Length;
        }

        /// <summary>
        /// 初始化文件流
        /// </summary>
        void InitFileStream()
        {
            FileMode mode = isResumeable ? FileMode.Append : FileMode.OpenOrCreate;
            try
            {
                _fileStream = new FileStream(_tempSavePath, mode, FileAccess.Write, FileShare.None);
                if (isResumeable)
                {
                    //断点续传的话，则先记录已下载的文件尺寸
                    downloadedSize = _fileStream.Length;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
                _fileStream = null;
            }
        }

        protected override void ReceiveContentLengthHeader(ulong contentLength)
        {
            //已收到过长度(iOS下可能重复收到)，或者长度为0，则直接返回
            if (0 == contentLength || totalSize > 0)
            {
                //Debug.Log($"重复收到ReceiveContentLengthHeader:{totalSize}");
                return;
            }
            totalSize = (long)contentLength + downloadedSize;
            Debug.Log($"[HttpDownloader] 下载的文件:{Path.GetFileName(savePath)} , size:{totalSize}");
            onReceivedHeaders?.Invoke();
        }

        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (IsDisposed)
            {
                return false;
            }

            if (data == null || data.Length == 0)
            {
                return false;
            }

            if (null == _fileStream)
            {
                //尝试初始化FileStream
                if (_request.responseCode >= 200 && _request.responseCode < 300)
                {
                    // 收到正常响应数据
                    // 在此处理数据
                    InitFileStream();
                }
                else
                {
                    // 收到错误信息
                    Debug.LogError($"[HttpDownloader] 收到的数据为错误信息, responseCode:{_request.responseCode}");
                    // 在此处理错误
                    return false;
                }
            }

            if (_fileStream == null || _fileStream.CanWrite == false)
            {
                return false;
            }

            try
            {
                _fileStream.Write(data, 0, dataLength);
            }
            catch
            {
                return false;
            }
            downloadedSize += dataLength;
            progress = (float)downloadedSize / totalSize;

            //Debug.Log($"下载到数据大小:{dataLength} 完成度:{GetProgress()} 已下载内容大小:{downloadedSize}/{totalSize}");

            onReceivedData?.Invoke(dataLength);

            return true;
        }

        protected override void CompleteContent()
        {
            CloseFileStream();
            if (File.Exists(_tempSavePath))
            {
                MoveFile(_tempSavePath, savePath, true);
            }
        }

        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="targetPath"></param>
        public static void MoveFile(string sourcePath, string targetPath, bool isOverride = false)
        {            
            if (isOverride && File.Exists(targetPath))
            {
                File.Delete(targetPath);
            }
            File.Move(sourcePath, targetPath);
        }

        protected override float GetProgress()
        {
            return progress;
        }

        protected override byte[] GetData()
        {
            if (_fileStream != null)
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

        /// <summary>
        /// 停止下载，并销毁
        /// </summary>
        /// <param name="isCleanTmepFile">是否清理已下载的临时文件</param>
        public void DisposeSafely(bool isCleanTmepFile)
        {
            CloseFileStream();

            if (isCleanTmepFile && File.Exists(_tempSavePath))
            {
                File.Delete(_tempSavePath);
            }

            Dispose();
            IsDisposed = true;
        }

        void CloseFileStream()
        {
            if (null != _fileStream)
            {
                _fileStream.Flush();
                _fileStream.Close();
                _fileStream.Dispose();
                _fileStream = null;
            }
        }
    }
}
