using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Zero
{
    public class BaseUpdater
    {
        public delegate void UpdateProgress(long loadedSize, long totalSize);

        /// <summary>
        /// 创始器执行完毕
        /// </summary>
        public event Action<BaseUpdater> onComplete;

        /// <summary>
        /// 进度更新
        /// </summary>
        public event UpdateProgress onProgress;

        /// <summary>
        /// 创始器错误，null表示没有错误
        /// </summary>
        public string error { get; private set; } = null;

        internal bool IsUpdating { get; private set; }
        
        protected CancellationToken CancelToken { get; private set; }

        public virtual void Start()
        {
            if (IsUpdating)
            {
                throw new Exception("Updater正在执行中!");
            }
            IsUpdating = true;
        }

        /// <summary>
        /// 修改为异步执行
        /// </summary>
        /// <param name="onProgress"></param>
        /// <param name="cancelToken"></param>
        /// <returns></returns>
        public UniTask<string> StartAsync(UpdateProgress onProgress = null, CancellationToken cancelToken = default)
        {
            CancelToken = cancelToken;
            var tcs = new UniTaskCompletionSource<string>();
            this.onComplete += OnComplete;
            this.onProgress += onProgress;
            Start();
            
            void OnComplete(BaseUpdater obj)
            {
                this.onComplete -= OnComplete;
                this.onProgress -= onProgress;
                tcs.TrySetResult(error);
            }

            return tcs.Task;
        }

        /// <summary>
        /// 创始器结束时调用，触发onComplete事件
        /// </summary>
        /// <param name="error"></param>
        protected virtual void End(string error = null)
        {
            IsUpdating = false;
            this.error = error;
            onComplete?.Invoke(this);
            CancelToken = CancellationToken.None;
        }

        protected virtual void Progress(long loadedSize, long totalSize)
        {
            onProgress?.Invoke(loadedSize, totalSize);
        }
    }
}
