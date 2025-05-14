using Jing;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Zero;
using Zero.Obsolete;

namespace Zero.Obsolete
{
    /// <summary>
    /// 热更Files管理工具。
    /// 所有传入的路径，都应该是相对于files热更目录下的相对路径。
    /// </summary>
    [Obsolete("请使用Zero.Res类")]
    public static class HotFilesMgr
    {
        public static byte[] LoadBytes(string path)
        {
            return LoadBytesAsync(path).GetAwaiter().GetResult();
        }

        public static async UniTask<byte[]> LoadBytesAsync(string path, Action<float> onProgress = null, CancellationToken cancellationToken = default)
        {
            var bytes = await HotRes.Load(GetResPath(path), onProgress, cancellationToken);
            return bytes;
        }

        public static async UniTask<string> LoadTextAsync(string path, Action<float> onProgress = null, CancellationToken cancellationToken = default)
        {
            var text = await HotRes.LoadString(GetResPath(path), onProgress, cancellationToken);
            return text;
        }

        public static string GetResPath(string path)
        {
            return FileUtility.CombinePaths(ZeroConst.FILES_DIR_NAME, path);
        }

        public static string GetAbsolutePath(string path)
        {
            return HotRes.GetAbsolutePath(GetResPath(path));
        }
    }
}