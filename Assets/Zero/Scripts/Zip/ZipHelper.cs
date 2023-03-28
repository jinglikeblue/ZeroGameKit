using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Zero
{
    /// <summary>
    /// Zip压缩/解压助手
    /// </summary>
    public class ZipHelper
    {
        public struct ProgressInfo
        {
            /// <summary>
            /// 处理的文件名称
            /// </summary>
            public string fileName;

            /// <summary>
            /// 完成进度
            /// </summary>
            public float progress;

            /// <summary>
            /// 已处理的字节数
            /// </summary>
            public long processedSize;

            /// <summary>
            /// 要处理的字节数
            /// </summary>
            public long totalSize;

            public override string ToString()
            {
                return $"[{fileName}] {progress}({processedSize}/{totalSize})";
            }
        }

        const string CANCEL_ERROR = "Canceled";

        /// <summary>
        /// 进度
        /// </summary>
        public ProgressInfo progressInfo { get; private set; }

        /// <summary>
        /// 是否完成
        /// </summary>
        public bool isDone { get; private set; } = false;

        /// <summary>
        /// 错误内容
        /// </summary>
        public string error { get; private set; } = null;

        /// <summary>
        /// 是否销毁
        /// </summary>
        public bool isDisposed { get; private set; } = false;

        /// <summary>
        /// 压缩文件名称
        /// </summary>
        public string zipFileName { get; private set; } = null;

        /// <summary>
        /// 对应的文件目录
        /// </summary>
        public string processDirectory { get; private set; } = null;

        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; private set; } = null;
        
        FastZip _zip;

        /// <summary>
        /// 是否取消了处理
        /// </summary>
        public bool isCanceled { get; private set; } = false;

        public ZipHelper(string password = null)
        {
            this.password = password;
        }

        private void OnProgress(object sender, ProgressEventArgs e)
        {
            ProgressInfo progressInfo;
            progressInfo.fileName = e.Name;
            progressInfo.progress = e.PercentComplete;
            progressInfo.processedSize = e.Processed;
            progressInfo.totalSize = e.Target;
            this.progressInfo = progressInfo;

            if(isCanceled)
            {
                //取消了压缩/解压
                e.ContinueRunning = false;
            }
        }

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="zipFileName"></param>
        /// <param name="sourceDirectory"></param>
        public void Compress(string zipFileName, string sourceDirectory)
        {
            Process(zipFileName, sourceDirectory, true);
        }

        /// <summary>
        /// 异步压缩文件
        /// </summary>
        /// <param name="zipFileName"></param>
        /// <param name="sourceDirectory"></param>
        public void CompressAsync(string zipFileName, string sourceDirectory)
        {
            Task.Run(() =>
            {
                ProcessAsync(zipFileName, sourceDirectory, true);
            });
        }

        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="zipFileName"></param>
        /// <param name="targetDirectory"></param>
        public void Uncompress(string zipFileName, string targetDirectory)
        {
            Process(zipFileName, targetDirectory, false);
        }

        public void UncompressAsync(string zipFileName, string targetDirectory)
        {
            Task.Run(() =>
            {
                ProcessAsync(zipFileName, targetDirectory, false);
            });
        }

        void ProcessAsync(string zipFileName, string processDirectory, bool isCompress)
        {
            var events = new FastZipEvents();
            var tn = new TimeSpan(TimeSpan.TicksPerSecond);
            events.ProgressInterval = tn;
            events.Progress += OnProgress;

            Process(zipFileName, processDirectory, isCompress, events);

            events.Progress -= OnProgress;

            if(isCanceled)
            {
                //清理文件
                if (isCompress)
                {
                    File.Delete(zipFileName);
                }
                else
                {
                    Directory.Delete(processDirectory, true);
                }
            }
        }

        void Process(string zipFileName, string processDirectory, bool isCompress, FastZipEvents events = null)
        {
            if(_zip != null)
            {
                throw new Exception($"实例化新的ZipHelper以进行新的压缩/解压缩任务");
            }

            try
            {
                _zip = events == null ? new FastZip() : new FastZip(events);
                _zip.Password = password;

                this.zipFileName = zipFileName;
                this.processDirectory = processDirectory;
                if (isCompress)
                {
                    _zip.CreateZip(zipFileName, processDirectory, true, "");
                }
                else
                {
                    _zip.ExtractZip(zipFileName, processDirectory, FastZip.Overwrite.Always, null, "", "", true);
                }                            
            }
            catch (Exception e)
            {
                error = e.Message;
            }

            if (isCanceled)
            {
                error = CANCEL_ERROR;
            }

            isDone = true;
        }

        /// <summary>
        /// 取消操作，并清理缓存(仅适用于异步模式)
        /// </summary>
        public void Cancel()
        {
            isCanceled = true;
        }
    }
}
