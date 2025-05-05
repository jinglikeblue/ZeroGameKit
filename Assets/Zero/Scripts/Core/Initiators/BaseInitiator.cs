using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Zero
{
    public abstract class BaseInitiator
    {
        public delegate void InitiatorProgress(long loadedSize, long totalSize);

        /// <summary>
        /// 创始器执行完毕
        /// </summary>
        public event Action<BaseInitiator> onComplete;
        
        /// <summary>
        /// 进度更新
        /// </summary>
        public event InitiatorProgress onProgress;

        /// <summary>
        /// 创始器错误，null表示没有错误
        /// </summary>
        public string error { get; private set; } = null;

        internal bool IsStarted { get; private set; }

        internal virtual void Start()
        {
            if (IsStarted)
            {
                throw new Exception("Initiator只能Start一次!");
            }
            IsStarted = true;
        }

        internal virtual async UniTask StartAsync()
        {
            if (IsStarted)
            {
                throw new Exception("Initiator只能Start一次!");
            }
            IsStarted = true;
        }

        /// <summary>
        /// 创始器结束时调用，触发onComplete事件
        /// </summary>
        /// <param name="error"></param>
        protected virtual void End(string error = null)
        {
            if (null != error)
            {
                Debug.LogError($"[{GetType().Name}] {error}");
            }
            this.error = error;
            onComplete?.Invoke(this);
        }

        protected virtual void Progress(long loadedSize, long totalSize)
        {
            onProgress?.Invoke(loadedSize, totalSize);
        }
    }
}
