using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Jing;
using UnityEngine;
using UnityEngine.Networking;

namespace Zero
{
    /// <summary>
    /// 资源操作类。
    /// 资源路径格式和res.json中保存的一致。
    /// </summary>
    public static class Res
    {
        /// <summary>
        /// 异步操作进度委托
        /// </summary>
        public delegate void ProgressDelegate(float progress, long loadedSize, long totalSize);

        ///  <summary>
        /// 检查资源是否有更新
        /// </summary>
        public static bool CheckUpdate(string path)
        {
            if (!Runtime.IsHotResEnable)
            {
                return false;
            }

            var isUpdateEnable = CheckNewVersion(path);
            if (false == isUpdateEnable)
            {
                //如果是AB，还要查依赖是否需要更新
                if (path.EndsWith(ZeroConst.AB_EXTENSION))
                {
                    var abdepends = ResMgr.GetDepends(path);
                    foreach (var ab in abdepends)
                    {
                        isUpdateEnable = CheckNewVersion(ResMgr.AddRootFolder(ab));
                        if (isUpdateEnable)
                        {
                            break;
                        }
                    }
                }
            }

            return isUpdateEnable;
        }

        /// <summary>
        /// 异步更新资源
        /// </summary>
        /// <param name="resPath"></param>
        /// <param name="onProgress"></param>
        /// <returns>错误码： null表示更新成功</returns>
        public static async UniTask<string> Update(string resPath, ProgressDelegate onProgress = null)
        {
            var updater = new HotResUpdater(resPath);
            string errInfo = await updater.StartAsync((loaded, total) => { onProgress?.Invoke(CalculateProgress(loaded, total), loaded, total); });
            return errInfo;
        }

        /// <summary>
        /// 异步更新资源组
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="onProgress"></param>
        /// <returns></returns>
        public static async UniTask<string> UpdateGroup(string[] groups, ProgressDelegate onProgress = null)
        {
            var updater = new HotResUpdater(groups);
            string errInfo = await updater.StartAsync((loaded, total) => { onProgress?.Invoke(CalculateProgress(loaded, total), loaded, total); });
            return errInfo;
        }

        /// <summary>
        /// 计算进度
        /// </summary>
        /// <param name="loadedSize"></param>
        /// <param name="totalSize"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float CalculateProgress(long loadedSize, long totalSize)
        {
            if (loadedSize < 0 || totalSize <= 0)
            {
                return 0;
            }

            var dProgress = (double)loadedSize / totalSize;
            var progress = Math.Clamp((float)dProgress, 0, 1);
            return progress;
        }

        /// <summary>
        /// 计算已加载大小
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="totalSize"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long CalculateLoadedSize(float progress, long totalSize)
        {
            if (progress < 0 || totalSize <= 0)
            {
                return 0;
            }

            var loaded = (double)progress * totalSize;
            var loadedSize = Math.Clamp((long)loaded, 0, totalSize);
            return loadedSize;
        }

        /// <summary>
        /// 检查文件是否有新的网络版本
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool CheckNewVersion(string resPath)
        {
            string localVer = Runtime.localResVer.GetVer(resPath);
            var newVer = Runtime.netResVer.GetVer(resPath);
            return localVer != newVer;
        }

        /// <summary>
        /// 检查资源是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool Exist(string path)
        {
            bool isExist = false;

            if (Runtime.IsUseAssetBundle)
            {
                //如果支持热更，则先检查热更目录
                if (Runtime.IsHotResEnable)
                {
                    isExist = CheckPersistentExist(path);
                }
            }
            else
            {
                isExist = CheckProjectExist(path);
            }

            //如果前面没有找到bytes，则尝试从StreamingAssets下获取
            if (false == isExist)
            {
                isExist = CheckStreamingAssetsExist(path);
            }

            return isExist;
        }

        /// <summary>
        /// 异步检查资源是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async UniTask<bool> ExistAsync(string path)
        {
            bool isExist = false;

            if (Runtime.IsUseAssetBundle)
            {
                //如果支持热更，则先检查热更目录
                if (Runtime.IsHotResEnable)
                {
                    isExist = CheckPersistentExist(path);
                }
            }
            else
            {
                isExist = CheckProjectExist(path);
            }

            //如果前面没有找到bytes，则尝试从StreamingAssets下获取
            if (false == isExist)
            {
                isExist = await CheckStreamingAssetsExistAsync(path);
            }

            return isExist;
        }

        /// <summary>
        /// 检查热更目录是否存在文件
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool CheckPersistentExist(string resPath)
        {
            //检查热更目录
            var path = GetPersistentPath(resPath);
            return File.Exists(path);
        }

        /// <summary>
        /// 检查工程目录是否存在文件
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool CheckProjectExist(string resPath)
        {
            var path = GetProjectPath(resPath);
            if (null != path)
            {
                return File.Exists(path);
            }

            return false;
        }

        /// <summary>
        /// 检查热更目录是否存在文件
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool CheckStreamingAssetsExist(string resPath)
        {
            var path = GetStreamingAssetsPath(resPath);
            return StreamingAssetsUtility.CheckFileExist(path);
        }

        /// <summary>
        /// 异步检查热更目录是否存在文件
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        private static async UniTask<bool> CheckStreamingAssetsExistAsync(string path)
        {
            path = GetStreamingAssetsPath(path);
            return await StreamingAssetsUtility.CheckFileExistAsync(path);
        }

        /// <summary>
        /// 获取资源的绝对路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string AbsolutePath(string path)
        {
            if (Runtime.IsUseAssetBundle)
            {
                //如果支持热更，则先检查热更目录
                if (Runtime.IsHotResEnable)
                {
                    if (CheckPersistentExist(path))
                    {
                        return GetPersistentPath(path);
                    }
                }
            }
            else
            {
                if (CheckProjectExist(path))
                {
                    return GetProjectPath(path);
                }
            }

            //如果前面没有找到，则尝试从StreamingAssets下检查
            var isExist = CheckStreamingAssetsExist(path);
            if (isExist)
            {
                return GetStreamingAssetsPath(path);
            }

            return null;
        }

        /// <summary>
        /// 加载资源
        /// 支持AssetBundle中的资源加载
        /// 支持AB文件加载
        /// 支持Files目录资源加载
        /// </summary>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Load<T>(string path) where T : class
        {
            //普通文件
            if (typeof(T) == typeof(byte[]))
            {
                var bytes = LoadBytes(path);
                return bytes as T;
            }

            //文本文件
            if (typeof(T) == typeof(string))
            {
                var bytes = LoadBytes(path);
                var text = Encoding.UTF8.GetString(bytes);
                return text as T;
            }

            //AB文件
            if (typeof(T) == typeof(AssetBundle))
            {
                return ResMgr.TryLoadAssetBundle(path) as T;
            }

            //Unity Asset
            if (typeof(T).IsSubclassOf(typeof(UnityEngine.Object)))
            {
                var asset = ResMgr.Load(path);
                return asset as T;
            }

            return null;
        }

        /// <summary>
        /// 异步加载资源
        /// 支持AssetBundle中的资源加载
        /// 支持AB文件加载
        /// 支持Files目录资源加载
        /// </summary>
        /// <param name="path"></param>
        /// <param name="onProgress"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async UniTask<T> LoadAsync<T>(string path, ProgressDelegate onProgress = null, CancellationToken cancellationToken = default) where T : class
        {
            //普通文件
            if (typeof(T) == typeof(byte[]))
            {
                var bytes = await LoadBytesAsync(path, onProgress, cancellationToken);
                return bytes as T;
            }

            //文本文件
            if (typeof(T) == typeof(string))
            {
                var bytes = await LoadBytesAsync(path, onProgress, cancellationToken);
                var text = Encoding.UTF8.GetString(bytes);
                return text as T;
            }

            //AB文件
            if (typeof(T) == typeof(AssetBundle))
            {
                return ResMgr.TryLoadAssetBundle(path) as T;
            }

            //Unity Asset
            if (typeof(T).IsSubclassOf(typeof(UnityEngine.Object)))
            {
                var asset = await ResMgr.LoadAsync(path, null, f => onProgress?.Invoke(f, CalculateLoadedSize(f, 100), 100));
                return asset as T;
            }

            return null;
        }

        /// <summary>
        /// 加载二进制数据
        /// </summary>
        /// <param name="path"></param>
        /// <param name="onProgress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static byte[] LoadBytes(string path)
        {
            byte[] bytes = null;

            if (Runtime.IsUseAssetBundle)
            {
                //如果支持热更，则先检查热更目录
                if (Runtime.IsHotResEnable)
                {
                    bytes = LoadFromPersistent(path);
                }
            }
            else
            {
                bytes = LoadFromProject(path);
            }

            //如果前面没有找到bytes，则尝试从StreamingAssets下获取
            if (null == bytes)
            {
                bytes = LoadFromStreamingAssets(path);
            }

            return bytes;
        }

        /// <summary>
        /// 异步加载二进制数据
        /// </summary>
        /// <param name="path"></param>
        /// <param name="onProgress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async UniTask<byte[]> LoadBytesAsync(string path, ProgressDelegate onProgress = null, CancellationToken cancellationToken = default)
        {
            onProgress?.Invoke(0, 0, 100);

            byte[] bytes = null;

            if (Runtime.IsUseAssetBundle)
            {
                //如果支持热更，则先检查热更目录
                if (Runtime.IsHotResEnable)
                {
                    bytes = await LoadFromPersistentAsync(path, cancellationToken);
                }
            }
            else
            {
                bytes = await LoadFromProjectAsync(path, cancellationToken);
            }

            //如果前面没有找到bytes，则尝试从StreamingAssets下获取
            if (null == bytes)
            {
                bytes = await LoadFromStreamingAssetsAsync(path, onProgress, cancellationToken);
            }

            onProgress?.Invoke(1, 100, 100);
            return bytes;
        }

        /// <summary>
        /// 从工程目录加载字节数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte[] LoadFromProject(string path)
        {
            path = GetProjectPath(path);
            if (null != path && File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }

            return null;
        }

        /// <summary>
        /// 异步从工程目录加载字节数据
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async UniTask<byte[]> LoadFromProjectAsync(string path, CancellationToken cancellationToken = default)
        {
            byte[] bytes = null;
            path = GetProjectPath(path);
            if (null != path && File.Exists(path))
            {
                try
                {
                    bytes = await File.ReadAllBytesAsync(path, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    bytes = null;
                }
            }

            return bytes;
        }

        /// <summary>
        /// 从内嵌资源加载字节数据
        /// </summary>
        /// <param name="path"></param>
        /// <param name="onProgress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static byte[] LoadFromStreamingAssets(string path)
        {
            path = GetStreamingAssetsPath(path);
            using var uwr = UnityWebRequest.Get(path);
            uwr.SendWebRequest();
            while (false == uwr.isDone)
            {
                //Thread Block!
            }

            if (uwr.error != null)
            {
                return null;
            }

            var bytes = uwr.downloadHandler.data;

            return bytes;
        }

        /// <summary>
        /// 从内嵌资源加载字节数据
        /// </summary>
        /// <param name="path"></param>
        /// <param name="onProgress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async UniTask<byte[]> LoadFromStreamingAssetsAsync(string path, ProgressDelegate onProgress = null, CancellationToken cancellationToken = default)
        {
            path = GetStreamingAssetsPath(path);
            using var uwr = UnityWebRequest.Get(path);
            uwr.SendWebRequest();
            while (false == uwr.isDone)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    uwr.Abort();
                    continue;
                }

                onProgress?.Invoke(uwr.downloadProgress, CalculateLoadedSize(uwr.downloadProgress, 100), 100);
                await UniTask.NextFrame();
            }

            if (uwr.error != null)
            {
                return null;
            }

            var bytes = uwr.downloadHandler.data;

            return bytes;
        }

        /// <summary>
        /// 从热更目录加载字节数据
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte[] LoadFromPersistent(string path)
        {
            //检查热更目录
            path = GetPersistentPath(path);
            if (File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }

            return null;
        }

        /// <summary>
        /// 异步从热更目录加载字节数据
        /// </summary>
        /// <param name="path"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private static async UniTask<byte[]> LoadFromPersistentAsync(string path, CancellationToken cancellationToken = default)
        {
            //检查热更目录
            byte[] bytes = null;
            path = GetPersistentPath(path);
            if (null != path && File.Exists(path))
            {
                try
                {
                    bytes = await File.ReadAllBytesAsync(path, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    bytes = null;
                }
            }

            return bytes;
        }

        /// <summary>
        /// 获取热更下载资源本地文件路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetPersistentPath(string path)
        {
            path = TransformToHotPath(path);
            return FileUtility.CombinePaths(ZeroConst.WWW_RES_PERSISTENT_DATA_PATH, path);
        }

        /// <summary>
        /// 获取内嵌资源文件路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetStreamingAssetsPath(string path)
        {
            path = TransformToHotPath(path);
            return FileUtility.CombinePaths(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW, path);
        }

        /// <summary>
        /// 获取Editor下，在Assets目录中的文件路径。
        /// 目前仅files文件夹下的资源可以获取。因为其它的资源只有AssetBundle模式下才会存在。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetProjectPath(string path)
        {
            //Editor下转换为从Project的目录下获取文件
            if (path.StartsWith(ZeroConst.FILES_DIR_NAME))
            {
                // 替换起始目录为Assets下的目录
                return FileUtility.CombinePaths(ZeroConst.HOT_FILES_ROOT_DIR, path.Substring(ZeroConst.FILES_DIR_NAME.Length));
            }

            //其它不支持的路径，直接返回null
            return null;
        }

        /// <summary>
        /// 将路径转换为热更目录下的路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string TransformToHotPath(string path)
        {
            if (path.StartsWith(ZeroConst.HOT_RESOURCES_ROOT_DIR))
            {
                return path.ReplaceAt(ZeroConst.HOT_RESOURCES_ROOT_DIR, ZeroConst.AB_DIR_NAME);
            }
            
            if (path.StartsWith(ZeroConst.HOT_FILES_ROOT_DIR))
            {
                return path.ReplaceAt(ZeroConst.HOT_FILES_ROOT_DIR, ZeroConst.FILES_DIR_NAME);
            }

            return path;
        }
    }
}