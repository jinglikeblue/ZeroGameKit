using System;
using Cysharp.Threading.Tasks;
using Jing;
using UnityEngine;
using UnityEngine.Networking;

namespace Zero
{
    /// <summary>
    /// WebGL核心类
    /// </summary>
    public static class WebGL
    {
        /// <summary>
        /// 当前环境是否是WebGL
        /// </summary>
        public static readonly bool IsEnvironmentWebGL = false;

        /// <summary>
        /// Manifest.ab
        /// </summary>
        public static AssetBundle ManifestAssetBundle { get; private set; }

        static WebGL()
        {
#if UNITY_WEBGL
            IsEnvironmentWebGL = true;
#endif
        }

        /// <summary>
        /// 如果是WebGL环境则抛出异常
        /// </summary>
        public static void ThrowErrorIfInWebGL(string message = null)
        {
            if (string.IsNullOrEmpty(message))
            {
                message = "当前逻辑无法在WebGL环境执行!";
            }

            throw new Exception(message);
        }

        /// <summary>
        /// 预载Manifest.ab文件
        /// </summary>
        public static async UniTask PreloadManifestAssetBundle()
        {
            Debug.Log($"[Zero][WebGL] 预载Manifest.ab文件...");
            var path = FileUtility.CombinePaths(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW, ZeroConst.AB_DIR_NAME, ZeroConst.MANIFEST_FILE_NAME + ZeroConst.AB_EXTENSION);
            ManifestAssetBundle = await RequestAssetBundle(path);
        }

        /// <summary>
        /// 请求AssetBundle
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async UniTask<AssetBundle> RequestAssetBundle(string path, Action<float> onProgress = null, Action<string> onError = null)
        {
            Debug.Log($"[Zero][WebGL][AssetBundle] 请求AssetBundle: {path}");

            // #region 获取文件大小
            //
            // long fileSize = 0;
            // using (UnityWebRequest headRequest = UnityWebRequest.Head(path))
            // {
            //     await headRequest.SendWebRequest().ToUniTask();
            //     string contentLength = headRequest.GetResponseHeader("Content-Length");
            //     if (long.TryParse(contentLength, out fileSize))
            //     {
            //         Debug.Log($"[Zero][WebGL][AssetBundle] 文件大小: {fileSize}, Path:{path}");
            //     }
            // }
            //
            // #endregion

            AssetBundle ab = null;
            using (var request = UnityWebRequestAssetBundle.GetAssetBundle(path))
            {
                onProgress?.Invoke(0);
                request.SendWebRequest();
                while (!request.isDone)
                {
                    onProgress?.Invoke(request.downloadProgress);
                    await UniTask.NextFrame();
                }

                onProgress?.Invoke(01);
                if (request.error != null)
                {
                    //出错了
                    onError?.Invoke(request.error);
                }

                ab = DownloadHandlerAssetBundle.GetContent(request);

                if (!ab)
                {
                    onError?.Invoke($"[Zero][WebGL][AssetBundle] 错误的AssetBundle：{path}");
                }
            }

            return ab;
        }
    }
}