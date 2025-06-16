using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        /// 异步操作进度委托
        /// </summary>
        public delegate void ProgressDelegate(float progress);

        /// <summary>
        /// 当前环境是否是WebGL
        /// </summary>
        public static readonly bool IsEnvironmentWebGL = false;

        /// <summary>
        /// Manifest.ab
        /// </summary>
        public static AssetBundle ManifestAssetBundle { get; private set; }

        const string AssetBundleFolder = ZeroConst.AB_DIR_NAME + "/";

        private static Dictionary<string, AssetBundle> _pathToAssetBundleDict = new Dictionary<string, AssetBundle>();
        private static Dictionary<string, byte[]> _pathToFileDict = new Dictionary<string, byte[]>();
        private static Dictionary<string, HashSet<ProgressDelegate>> _preparingPathToDelegateSetDict = new Dictionary<string, HashSet<ProgressDelegate>>();

        static WebGL()
        {
#if UNITY_WEBGL
            IsEnvironmentWebGL = true;
#endif

#if UNITY_EDITOR
            if (false == LauncherSetting.LoadLauncherSettingDataFromResources().isUseAssetBundle)
            {
                IsEnvironmentWebGL = false;
            }
#endif
        }

        /// <summary>
        /// 注册一个正在预加载的路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pd"></param>
        private static void RegisterPreparingPath(string path, ProgressDelegate pd)
        {
            if (_preparingPathToDelegateSetDict.TryGetValue(path, out var set))
            {
                set.Add(pd);
            }
            else
            {
                set = new HashSet<ProgressDelegate> { pd };
                _preparingPathToDelegateSetDict.Add(path, set);
            }
        }

        /// <summary>
        /// 注销一个预加载路径
        /// </summary>
        /// <param name="path"></param>
        private static void UnregisterPreparingPath(string path)
        {
            _preparingPathToDelegateSetDict.Remove(path);
        }

        /// <summary>
        /// 更新正在预加载的进度
        /// </summary>
        /// <param name="progress"></param>
        private static void UpdatePreparingPath(string path, float progress)
        {
            if (_preparingPathToDelegateSetDict.TryGetValue(path, out var set))
            {
                foreach (var pd in set)
                {
                    pd?.Invoke(progress);
                }
            }
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
        public static string MakeAbsolutePath(string path, EResType resType = EResType.Unknown)
        {
            var hotPath = Res.TransformToHotPath(path, resType);
            var item = Runtime.resVer.Get(hotPath);
            if (null == item)
            {
                Debug.LogError($"[Zero][WebGL] 无法获取资源版本信息： {hotPath}");
            }

            return GetRequestPath(item);
        }

        /// <summary>
        /// 预载Manifest.ab文件
        /// </summary>
        public static async UniTask PrepareManifestAssetBundle()
        {
            Debug.Log($"[Zero][WebGL] 预载Manifest.ab文件...");
            var abName = Res.TransformToHotPath(ZeroConst.MANIFEST_FILE_NAME + ZeroConst.AB_EXTENSION, EResType.Asset);
            var item = Runtime.resVer.Get(abName);
            ManifestAssetBundle = await PrepareAssetBundle(item);
        }

        /// <summary>
        /// AssetBundle。
        /// 注意：AssetBundle必须通过PreloadAssetBundles进行了预加载，才能成功获取。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isLogEnable"></param>
        /// <returns></returns>
        public static AssetBundle GetAssetBundle(string path, bool isLogEnable = false)
        {
            path = Res.TransformToHotPath(path, EResType.Asset);

            if (_pathToAssetBundleDict.ContainsKey(path) && !_pathToAssetBundleDict[path])
            {
                //清理被销毁的资源
                _pathToAssetBundleDict.Remove(path);
            }

            _pathToAssetBundleDict.TryGetValue(path, out var ab);
            if (isLogEnable)
            {
                if (ab)
                {
                    Debug.Log(LogColor.Green($"[Zero][WebGL] 获取AssetBundle成功: {path}"));
                }
                else
                {
                    Debug.LogError($"[Zero][WebGL] 获取AssetBundle失败: {path}");
                }
            }

            return ab;
        }

        /// <summary>
        /// 获取File。
        /// 注意：必须通过PreloadFiles进行了预加载，才能成功获取。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isLogEnable"></param>
        /// <returns></returns>
        public static byte[] GetFile(string path, bool isLogEnable = false)
        {
            path = Res.TransformToHotPath(path, EResType.File);
            _pathToFileDict.TryGetValue(path, out var file);
            if (isLogEnable)
            {
                if (file != null)
                {
                    Debug.Log(LogColor.Green($"[Zero][WebGL] 获取File成功: {path}"));
                }
                else
                {
                    Debug.LogError($"[Zero][WebGL] 获取File失败: {path}");
                }
            }

            return file;
        }

        /// <summary>
        /// 预载资源。
        /// WebGL环境下，File必须通过预载才能读取使用。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="onProgress"></param>
        public static async UniTask Prepare(string path, ProgressDelegate onProgress = null)
        {
            var item = Runtime.resVer.Get(path);
            await Prepare(item, onProgress);
        }

        /// <summary>
        /// 预载资源。
        /// WebGL环境下，File必须通过预载才能读取使用。
        /// </summary>
        /// <param name="item"></param>
        /// <param name="onProgress"></param>
        private static async UniTask Prepare(ResVerVO.Item item, ProgressDelegate onProgress = null)
        {
            var resType = Res.GetResType(item.name);
            switch (resType)
            {
                case EResType.File:
                    await PrepareFile(item, onProgress);
                    break;
                case EResType.Asset:
                    await PrepareAssetBundle(item, onProgress);
                    break;
            }
        }

        /// <summary>
        /// 预载二进制资源。WebGL环境下，File必须通过预载才能读取使用。
        /// TODO path还没下载完，重复请求就来了的情况
        /// </summary>
        /// <param name="path"></param>
        /// <param name="onProgress"></param>
        /// <returns></returns>
        private static async UniTask<byte[]> PrepareFile(ResVerVO.Item item, ProgressDelegate onProgress = null)
        {
            var cache = GetFile(item.name);
            if (cache != null)
            {
                onProgress?.Invoke(1);
                return cache;
            }

            RegisterPreparingPath(item.name, onProgress);
            try
            {
                //获取资源的版本信息
                var url = GetRequestPath(item);
                Debug.Log($"[Zero][WebGL][AssetBundle] 预载File: {url}");

                using (UnityWebRequest request = UnityWebRequest.Get(url))
                {
                    request.SendWebRequest();
                    while (!request.isDone)
                    {
                        UpdatePreparingPath(item.name, request.downloadProgress);
                        await UniTask.NextFrame();
                    }

                    UpdatePreparingPath(item.name, 1);

                    if (!string.IsNullOrEmpty(request.error))
                    {
                        throw new Exception(request.error);
                    }

                    cache = request.downloadHandler.data;
                    _pathToFileDict.Add(item.name, cache);
                }
            }
            finally
            {
                UnregisterPreparingPath(item.name);
            }


            return cache;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetRequestPath(ResVerVO.Item item)
        {
            var root = Runtime.IsHotResEnable ? Runtime.netResDir : ZeroConst.STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW;
            var path = FileUtility.CombinePaths(root, item.ToUrlWithVer());
            return path;
        }

        /// <summary>
        /// 请求AssetBundle
        /// </summary>
        /// <param name="item"></param>
        /// <param name="onProgress"></param>
        /// <returns></returns>
        private static async UniTask<AssetBundle> PrepareAssetBundle(ResVerVO.Item item, ProgressDelegate onProgress = null)
        {
            //TODO 思考：在这里检查依赖是否会更好，还是统一要求从Res.Prepare方法中预载资源
            // Assets.GetDepends(item.name);
            
            var cache = GetAssetBundle(item.name);
            if (cache)
            {
                onProgress?.Invoke(1);
                return cache;
            }

            RegisterPreparingPath(item.name, onProgress);
            try
            {
                var url = GetRequestPath(item);
                Debug.Log($"[Zero][WebGL][AssetBundle] 预载AssetBundle: {url}");

                using (var request = UnityWebRequestAssetBundle.GetAssetBundle(url))
                {
                    onProgress?.Invoke(0);
                    request.SendWebRequest();
                    while (!request.isDone)
                    {
                        UpdatePreparingPath(item.name, request.downloadProgress);
                        await UniTask.NextFrame();
                    }

                    UpdatePreparingPath(item.name, 1);
                    if (request.error != null)
                    {
                        //出错了
                        throw new Exception(request.error);
                    }

                    try
                    {
                        cache = DownloadHandlerAssetBundle.GetContent(request);
                        _pathToAssetBundleDict.Add(item.name, cache);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                        throw new Exception($"[Zero][WebGL][AssetBundle] 错误的AssetBundle：{url}");
                    }
                }
            }
            finally
            {
                UnregisterPreparingPath(item.name);
            }

            return cache;
        }
    }
}