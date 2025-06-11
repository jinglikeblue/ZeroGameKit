using System;
using System.Collections.Generic;
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

        const string AssetBundleFolder = ZeroConst.AB_DIR_NAME + "/";

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
                message = "当前代码无法在WebGL环境继续执行!请调整代码进行适配!";
            }

            throw new Exception(message);
        }

        /// <summary>
        /// 转换为绝对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string MakeAbsolutePath(string path)
        {
            if (path.StartsWith(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW))
            {
                return path;
            }

            if (path.StartsWith(AssetBundleFolder))
            {
                return FileUtility.CombinePaths(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW, path);
            }

            return FileUtility.CombinePaths(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW, ZeroConst.AB_DIR_NAME, path);
        }

        /// <summary>
        /// 转换为相对路径。[ab/]开头的路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string MakeRelativePath(string path)
        {
            if (path.StartsWith(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW))
            {
                return path.Remove(0, ZeroConst.STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW.Length + 1);
            }

            if (!path.StartsWith(AssetBundleFolder))
            {
                return FileUtility.CombinePaths(AssetBundleFolder, path);
            }

            return path;
        }

        /// <summary>
        /// 预载Manifest.ab文件
        /// </summary>
        public static async UniTask PreloadManifestAssetBundle()
        {
            Debug.Log($"[Zero][WebGL] 预载Manifest.ab文件...");
            var path = MakeAbsolutePath(ZeroConst.MANIFEST_FILE_NAME + ZeroConst.AB_EXTENSION);
            ManifestAssetBundle = await RequestAssetBundle(path);
        }

        private static Dictionary<string, AssetBundle> _pathToAssetBundleDict = new Dictionary<string, AssetBundle>();


        /// <summary>
        /// 获取AssetBundle。
        /// 注意：AssetBundle必须通过PreloadAssetBundles进行了预加载，才能获取。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static AssetBundle GetAssetBundle(string path)
        {
            path = MakeRelativePath(path);
            Debug.Log($"[Zero][WebGL] 获取AssetBundle: {path}");
            if (!_pathToAssetBundleDict.TryGetValue(path, out var ab))
            {
                Debug.LogError($"[Zero][WebGL] 获取AssetBundle失败，请确认是否进行了预加载。 Path: {path}");
            }

            return ab;
        }

        /// <summary>
        /// 预载AssetBundle。WebGL环境下，AssetBundle必须通过预载才能使用。
        /// </summary>
        /// <param name="paths"></param>
        public static async UniTask PreloadAssetBundles(string[] paths)
        {
            Debug.Log($"[Zero][WebGL][AssetBundle] 预载AssetBundle: {string.Join(",", paths)}");

            //TODO 这里应该有预载进度

            foreach (var path in paths)
            {
                var ab = await RequestAssetBundle(path);
                _pathToAssetBundleDict.Add(MakeRelativePath(path), ab);
            }
        }

        /// <summary>
        /// 请求AssetBundle
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async UniTask<AssetBundle> RequestAssetBundle(string path, Action<float> onProgress = null, Action<string> onError = null)
        {
            path = MakeAbsolutePath(path);
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

                try
                {
                    ab = DownloadHandlerAssetBundle.GetContent(request);
                }
                catch (Exception e)
                {
                    ab = null;
                }

                if (!ab)
                {
                    onError?.Invoke($"[Zero][WebGL][AssetBundle] 错误的AssetBundle：{path}");
                }
            }

            return ab;
        }
    }
}