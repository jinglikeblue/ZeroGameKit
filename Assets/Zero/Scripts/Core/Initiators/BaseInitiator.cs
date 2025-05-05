using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Zero
{
    public abstract class BaseInitiator
    {
        public delegate void InitiatorProgress(long loadedSize, long totalSize);
        
        /// <summary>
        /// 进度更新
        /// </summary>
        // public event InitiatorProgress onProgress;

        // /// <summary>
        // /// 创始器错误，null表示没有错误
        // /// </summary>
        // public string error { get; private set; } = null;

        /// <summary>
        /// 异步执行启动器
        /// </summary>
        /// <param name="onProgress">进度回调方法</param>
        /// <returns>错误描述，null表示运行成功。</returns>
        internal abstract UniTask<string> StartAsync(InitiatorProgress onProgress = null);

        // /// <summary>
        // /// 创始器结束时调用，触发onComplete事件
        // /// </summary>
        // /// <param name="error"></param>
        // protected void End(string error = null)
        // {
        //     if (null != error)
        //     {
        //         Debug.LogError($"[{GetType().Name}] {error}");
        //     }
        //     this.error = error;
        //     onComplete?.Invoke(this);
        // }
        //
        // protected void Progress(long loadedSize, long totalSize)
        // {
        //     onProgress?.Invoke(loadedSize, totalSize);
        // }
    }
}
