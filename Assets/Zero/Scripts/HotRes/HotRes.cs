using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Jing;
using UnityEngine.Networking;

namespace Zero
{
    /// <summary>
    /// 热更资源文件操作类
    /// </summary>
    public static class HotRes
    {
        /// <summary>
        /// 获取热更下载资源本地文件路径
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetPersistentPath(string resPath)
        {
            return FileUtility.CombinePaths(ZeroConst.WWW_RES_PERSISTENT_DATA_PATH, resPath);
        }

        /// <summary>
        /// 获取内嵌资源文件路径
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetStreamingAssetsPath(string resPath)
        {
            return FileUtility.CombinePaths(ZeroConst.STREAMING_ASSETS_RES_DATA_PATH_FOR_WWW, resPath);
        }

        /// <summary>
        /// 获取Editor下，在Assets目录中的文件路径
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetProjectPath(string resPath)
        {
            //Editor下转换为从Project的目录下获取文件
            if (resPath.StartsWith(ZeroConst.FILES_DIR_NAME))
            {
                // 替换起始目录为Assets下的目录
                return FileUtility.CombinePaths(ZeroConst.HOT_FILES_ROOT_DIR, resPath.Substring(ZeroConst.FILES_DIR_NAME.Length));
            }

            //其它不支持的路径，直接返回null
            return null;
        }

        /// <summary>
        /// 异步检查热更资源文件是否存在
        /// </summary>
        /// <param name="resPath">热更资源的相对路径（相对于热更资源根目录，比如：「dll/scripts.dll」）</param>
        /// <returns></returns>
        public static async UniTask<bool> ExistAsync(string resPath)
        {
            bool isExist = false;

            if (Runtime.Ins.IsUseAssetBundle)
            {
                //如果支持热更，则先检查热更目录
                if (Runtime.Ins.IsHotResEnable)
                {
                    isExist = CheckPersistentExist(resPath);
                }
            }
            else
            {
                isExist = CheckProjectExist(resPath);
            }

            //如果前面没有找到bytes，则尝试从StreamingAssets下获取
            if (false == isExist)
            {
                isExist = await CheckStreamingAssetsExist(resPath);
            }

            return isExist;
        }

        /// <summary>
        /// 如果资源存在，则返回读取资源的绝对路径
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        public static string GetAbsolutePath(string resPath)
        {
            if (Runtime.Ins.IsUseAssetBundle)
            {
                //如果支持热更，则先检查热更目录
                if (Runtime.Ins.IsHotResEnable)
                {
                    if (CheckPersistentExist(resPath))
                    {
                        return GetPersistentPath(resPath);
                    }
                }
            }
            else
            {
                if (CheckProjectExist(resPath))
                {
                    return GetProjectPath(resPath);
                }
            }

            //如果前面没有找到，则尝试从StreamingAssets下检查
            var isExist = CheckStreamingAssetsExistSync(resPath);
            if (isExist)
            {
                return GetStreamingAssetsPath(resPath);
            }

            return null;
        }

        /// <summary>
        /// 如果资源存在，则返回读取资源的绝对路径
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        public static async UniTask<string> GetAbsolutePathAsync(string resPath)
        {
            if (Runtime.Ins.IsUseAssetBundle)
            {
                //如果支持热更，则先检查热更目录
                if (Runtime.Ins.IsHotResEnable)
                {
                    if (CheckPersistentExist(resPath))
                    {
                        return GetPersistentPath(resPath);
                    }
                }
            }
            else
            {
                if (CheckProjectExist(resPath))
                {
                    return GetProjectPath(resPath);
                }
            }

            //如果前面没有找到，则尝试从StreamingAssets下检查
            var isExist = await CheckStreamingAssetsExist(resPath);
            if (isExist)
            {
                return GetStreamingAssetsPath(resPath);
            }

            return null;
        }

        /// <summary>
        /// 加载热更资源文件
        /// </summary>
        /// <param name="resPath">热更资源的相对路径（相对于热更资源根目录，比如：「dll/scripts.dll」。）</param>
        /// <param name="onProgress">加载进度</param>
        /// <param name="cancellationToken">异步操作取消令牌</param>
        /// <returns></returns>
        public static async UniTask<byte[]> Load(string resPath, Action<float> onProgress = null, CancellationToken cancellationToken = default)
        {
            onProgress?.Invoke(0);

            byte[] bytes = null;

            if (Runtime.Ins.IsUseAssetBundle)
            {
                //如果支持热更，则先检查热更目录
                if (Runtime.Ins.IsHotResEnable)
                {
                    bytes = LoadFromPersistent(resPath);
                }
            }
            else
            {
                bytes = LoadFromProject(resPath);
            }

            //如果前面没有找到bytes，则尝试从StreamingAssets下获取
            if (null == bytes)
            {
                bytes = await LoadFromStreamingAssets(resPath, onProgress, cancellationToken);
            }

            onProgress?.Invoke(1);
            return bytes;
        }

        /// <summary>
        /// 以文本形式，加载热更资源文件
        /// </summary>
        /// <param name="resPath">热更资源的相对路径（相对于热更资源根目录，比如：「files/read_me.txt」。）</param>
        /// <param name="onProgress">加载进度</param>
        /// <param name="cancellationToken">异步操作取消令牌</param>
        /// <returns></returns>
        public static async UniTask<string> LoadString(string resPath, Action<float> onProgress = null, CancellationToken cancellationToken = default)
        {
            string str = null;
            var bytes = await Load(resPath, onProgress, cancellationToken);
            if (null != bytes)
            {
                str = Encoding.UTF8.GetString(bytes);
            }

            return str;
        }


        /// <summary>
        /// 检查热更目录是否存在文件
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckPersistentExist(string resPath)
        {
            //检查热更目录
            var path = GetPersistentPath(resPath);
            return File.Exists(path);
        }

        /// <summary>
        /// 从热更目录加载字节数据
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        public static byte[] LoadFromPersistent(string resPath)
        {
            //检查热更目录
            var path = GetPersistentPath(resPath);
            if (File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }

            return null;
        }

        /// <summary>
        /// 检查热更目录是否存在文件
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        public static bool CheckStreamingAssetsExistSync(string resPath)
        {
            var path = GetStreamingAssetsPath(resPath);
            return StreamingAssetsUtility.CheckFileExist(path);
        }

        /// <summary>
        /// 检查热更目录是否存在文件
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        public static UniTask<bool> CheckStreamingAssetsExist(string resPath)
        {
            var path = GetStreamingAssetsPath(resPath);
            return StreamingAssetsUtility.CheckFileExistAsync(path);
        }

        /// <summary>
        /// 从内嵌资源加载字节数据
        /// </summary>
        /// <param name="resPath"></param>
        /// <param name="onProgress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async UniTask<byte[]> LoadFromStreamingAssets(string resPath, Action<float> onProgress = null, CancellationToken cancellationToken = default)
        {
            var path = GetStreamingAssetsPath(resPath);
            using var uwr = UnityWebRequest.Get(path);
            uwr.SendWebRequest();
            while (false == uwr.isDone)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    uwr.Abort();
                    continue;
                }

                onProgress?.Invoke(uwr.downloadProgress);
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
        /// 检查工程目录是否存在文件
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckProjectExist(string resPath)
        {
            var path = GetProjectPath(resPath);
            if (null != path)
            {
                return File.Exists(path);
            }

            return false;
        }

        /// <summary>
        /// 从工程目录加载字节数据
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] LoadFromProject(string resPath)
        {
            var path = GetProjectPath(resPath);
            if (null != path && File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }

            return null;
        }

        /// <summary>
        /// 检查资源是否有更新
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        public static bool CheckUpdateEnable(string resPath)
        {
            if (!Runtime.Ins.IsHotResEnable)
            {
                return false;
            }

            var isUpdateEnable = CheckHasNewNetVersion(resPath);
            if (false == isUpdateEnable)
            {
                //如果是AB，还要查依赖是否需要更新
                if (resPath.EndsWith(ZeroConst.AB_EXTENSION))
                {
                    var abdepends = ResMgr.GetDepends(ResMgr.RemoveRootFolder(resPath));
                    foreach (var ab in abdepends)
                    {
                        isUpdateEnable = CheckHasNewNetVersion(ResMgr.AddRootFolder(ab));
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
        /// 检查文件是否有新的网络版本
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool CheckHasNewNetVersion(string resPath)
        {
            string localVer = Runtime.Ins.localResVer.GetVer(resPath);
            var newVer = Runtime.Ins.netResVer.GetVer(resPath);
            return localVer != newVer;
        }

        /// <summary>
        /// 更新资源
        /// </summary>
        /// <param name="resPath"></param>
        /// <param name="onProgress"></param>
        /// <returns>错误码： null表示更新成功</returns>
        public static async UniTask<string> Update(string resPath, BaseUpdater.UpdateProgress onProgress = null)
        {
            var updater = new HotResUpdater(resPath);
            string errInfo = await updater.StartAsync(onProgress);
            return errInfo;
        }

        /// <summary>
        /// 更新资源组
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="onProgress"></param>
        /// <returns></returns>
        public static async UniTask<string> UpdateGroup(string[] groups, BaseUpdater.UpdateProgress onProgress = null)
        {
            var updater = new HotResUpdater(groups);
            string errInfo = await updater.StartAsync(onProgress);
            return errInfo;
        }
    }
}